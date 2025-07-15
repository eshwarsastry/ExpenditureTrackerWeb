using ExpenditureTrackerWeb.AutoGen;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Enums;
using ExpenditureTrackerWeb.Shared.Mappers;
using ExpenditureTrackerWeb.Shared.OCR;
using System.Globalization;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface IImportDataService
    {
        public Task<bool> ImportDataAsync(IFormFile importFile, int userId);

        public Task<ExpenseDto> ExtractBillDataAsync(IFormFile importFile, int userId);

    }
    public class ImportDataService : IImportDataService
    {
        private readonly ITransactionTypeService transactionTypeService;
        private readonly ITransactionCategoryService transactionCategoryService;
        private readonly IExpensesService expensesService;
        private readonly IBillInformationExtractor billInformationExtractor;
        private readonly IExpensesMapper expensesMapper;
        private readonly IUserService userService;
        private readonly IImportCSVDataMapperAgent importCSVDataMapperAgent;
        public ImportDataService(ITransactionTypeService _transactionTypeService,
            ITransactionCategoryService _transactionCategoryService,
            IExpensesService _expensesService,
            IExpensesMapper _expensesMapper,
            IBillInformationExtractor _billInformationExtractor,
            IUserService _userService,
            IImportCSVDataMapperAgent _importCSVDataMapperAgent)
        {
            transactionTypeService = _transactionTypeService;
            transactionCategoryService = _transactionCategoryService;
            expensesService = _expensesService;
            expensesMapper = _expensesMapper;
            userService = _userService;
            billInformationExtractor = _billInformationExtractor;
            importCSVDataMapperAgent = _importCSVDataMapperAgent;
        }

        public async Task<bool> ImportDataAsync(IFormFile importFile, int userId)
        {
            var transactionTypes = await transactionTypeService.GetAll();
            var transactionCategories = await transactionCategoryService.GetAllByUserId(userId);

            var expensesDto = new List<ExpenseDto>();
            var newTransactionCategories = new List<CategoryDto>();

            if (importFile == null || importFile.Length == 0)
            {
                throw new ArgumentException("Import file is empty or null.");
            }

            using (var stream = importFile.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    string headerLine = await reader.ReadLineAsync();
                    if (headerLine == null)
                    {
                        throw new InvalidOperationException("CSV file is empty or does not contain a header.");
                    }
                    var headerColumns = headerLine.Split(',');

                    var categoryIndex = Array.FindIndex(headerColumns, h => h.Trim().Equals("Category", StringComparison.OrdinalIgnoreCase));
                    var amountIndex = Array.FindIndex(headerColumns, h => h.Trim().Equals("Amount", StringComparison.OrdinalIgnoreCase));
                    var transactionDate = Array.FindIndex(headerColumns, h => h.Trim().Equals("Date", StringComparison.OrdinalIgnoreCase));
                    var noteIndex = Array.FindIndex(headerColumns, h => h.Trim().Equals("Note", StringComparison.OrdinalIgnoreCase));

                    if (categoryIndex < 0)
                    {
                        throw new InvalidOperationException("CSV file does not contain a 'Category' column.");
                    }

                    while ((line = await reader.ReadLineAsync()) != null)
                    {

                        var columns = line.Split(',');
                        var categoryName = columns[categoryIndex].Trim();
                        var amountValue = columns.Length > amountIndex ? Decimal.Parse(columns[amountIndex].Trim(), CultureInfo.InvariantCulture) : 0;
                        var dateValue = columns.Length > transactionDate ? DateTime.Parse(columns[transactionDate].Trim()) : DateTime.Now;
                        var noteValue = columns.Length > noteIndex ? columns[noteIndex].Trim() : string.Empty;

                        if (!string.IsNullOrEmpty(categoryName)
                            && !transactionCategories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                            && !newTransactionCategories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
                        {
                            var categoryDto = new CategoryDto
                            {
                                User_Id = userId,
                                Name = categoryName,
                            };
                            newTransactionCategories.Add(categoryDto);
                        }
                        if (amountValue != 0)
                        {
                            var expenseDto = new ExpenseDto
                            {
                                User_Id = userId,
                                Amount = Math.Abs(amountValue),
                                TransactionDate = dateValue,
                                Note = noteValue,
                                Category_Id = transactionCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))?.Id ?? 0,
                                Category_Name = categoryName,
                            };
                            expensesDto.Add(expenseDto);
                        }

                    }
                }
            }

            // Map the new catgeories to their transaction types and add the new categories
            if (newTransactionCategories.Count > 0)
            {
                string inputText = await GetInputTextForAgentAsync(newTransactionCategories, transactionTypes.ToList());
                var categoryTransactionTypeMapperDtos = await importCSVDataMapperAgent.InitializeAgentsAsync(inputText);
                newTransactionCategories = GetTransactionTypeIdForCategory(categoryTransactionTypeMapperDtos, newTransactionCategories);
                
                await transactionCategoryService.AddBulkCategories(newTransactionCategories);
            }
            // Add the expenses
            if (expensesDto.Count > 0)
            {
                await expensesService.AddBulkExpenses(expensesDto);
            }

            return true;
        }

        public async Task<ExpenseDto> ExtractBillDataAsync(IFormFile imageFile, int userId)
        {
            var user = await userService.GetUserEntity(userId);

            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Import file is empty or null.");
            }

            // Read the file into a memory stream
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);

            // Get byte array
            var imageBytes = memoryStream.ToArray();

            var billInformationText = billInformationExtractor.BillInformationOCRExtractor(imageBytes);
            var userTransactionCategories = await transactionCategoryService.GetAllByUserId(userId);
            
            // Pass the base64 string to the agent
            var billDetailsExtractorAgent = new BillInformationAnalyserAgent();
            try
            {
                string listOfCategories = await GetInputTextForAgentAsync(userTransactionCategories.ToList());
                var extractedBillDetails = await billDetailsExtractorAgent.InitializeAgentsAsync(billInformationText, listOfCategories);
                if (extractedBillDetails == null)
                {
                    throw new InvalidOperationException("No bill details extracted from the image.");
                }
                var transactionCategory = await transactionCategoryService.GetByCategoryName(extractedBillDetails.Category, userId);
                if (transactionCategory == null)
                {
                    throw new InvalidOperationException($"Category '{extractedBillDetails.Category}' not found for user with ID {userId}.");
                }
                var result = expensesMapper.ToExpenseDto(extractedBillDetails, transactionCategory, user);
                return result;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the extraction process
                throw new InvalidOperationException("An error occurred while extracting bill details.", ex);
            }
        }


        private async Task<string> GetInputTextForAgentAsync(List<CategoryDto> newCatgories, List<TransactionTypeDto> transactionTypeDtos)
        {
            var categoryNames = newCatgories
                .Select(c => c.Name)
                .Distinct()
                .ToArray();

            var transactionTypeNames = transactionTypeDtos
                .Select(c => c.TransactionType)
                .Distinct()
                .ToArray();

            return $"Categories: [{string.Join(", ", categoryNames)}], Transaction_Types: [{string.Join(", ", transactionTypeNames)}]";
        }

        private async Task<string> GetInputTextForAgentAsync(List<CategoryDto> newCatgories)
        {
            var categoryNames = newCatgories
                .Select(c => c.Name)
                .Distinct()
                .ToArray();

            return $"Categories: [{string.Join(", ", categoryNames)}]]";
        }

        private List<CategoryDto> GetTransactionTypeIdForCategory(List<CategoryTransactionTypeMapperDto> categoryTransactionTypeMapperDtos,
            List<CategoryDto> newCatgories)
        {
            foreach (var newCategory in newCatgories)
            {
                var categoryMapper = categoryTransactionTypeMapperDtos.FirstOrDefault(c => c.CategoryName.Equals(newCategory.Name, StringComparison.OrdinalIgnoreCase));
                if (categoryMapper != null)
                {
                    newCategory.TransactionType_Name = categoryMapper.TransactionType;
                }
            }
            return newCatgories;
        }
    }
}
