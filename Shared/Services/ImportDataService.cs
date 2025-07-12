using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Enums;
using System.Globalization;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface IImportDataService
    {
        public Task<bool> ImportDataAsync(IFormFile importFile, int userId);
    }
    public class ImportDataService: IImportDataService
    {
        private readonly ITransactionTypeService transactionTypeService;
        private readonly ITransactionCategoryService transactionCategoryService;
        private readonly IExpensesService expensesService;

        public ImportDataService(ITransactionTypeService _transactionTypeService,
            ITransactionCategoryService _transactionCategoryService,
            IExpensesService _expensesService)
        {
            transactionTypeService = _transactionTypeService;
            transactionCategoryService = _transactionCategoryService;
            expensesService = _expensesService;
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
                                TransactionType_Id = amountValue > new Decimal(0.00) ?
                                transactionTypes.Where(transactionTypeEntity => transactionTypeEntity.Id == (int)TransactionTypeEnum.Income).First().Id :
                                transactionTypes.Where(transactionTypeEntity => transactionTypeEntity.Id == (int)TransactionTypeEnum.Expenditure).First().Id, 
                                // Assuming amount is positive for Income and negative for Expenditure
                            };
                            newTransactionCategories.Add(categoryDto);
                        }
                        if (amountValue != 0)
                        {
                            var expenseDto = new ExpenseDto
                            {
                                User_Id = userId,
                                Amount = amountValue,
                                TransactionDate = dateValue,
                                Note = noteValue,
                                Category_Id = transactionCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))?.Id ?? 0,
                                TransactionType_Id = amountValue > new Decimal(0.00) ?
                                    transactionTypes.Where(transactionTypeEntity => transactionTypeEntity.Id == (int)TransactionTypeEnum.Income).First().Id :
                                    transactionTypes.Where(transactionTypeEntity => transactionTypeEntity.Id == (int)TransactionTypeEnum.Expenditure).First().Id, // Assuming amount is positive for Income and negative for Expenditure
                            };
                            expensesDto.Add(expenseDto);
                        }
                        
                    }
                }
            }

            // Add the new categories
            if(newTransactionCategories.Count > 0)
            {
                await transactionCategoryService.AddBulkCategories(newTransactionCategories);
            }
            // Add the expenses
            if (expensesDto.Count > 0)
            {
                await expensesService.AddBulkExpenses(expensesDto);
            }

            return true;
        }
    }
}
