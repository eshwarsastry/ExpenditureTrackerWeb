using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using ExpenditureTrackerWeb.Shared.Dto.Predictor;
using ExpenditureTrackerWeb.Shared.Predictor;
using ExpenditureTrackerWeb.Shared.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ExpenditureTrackerWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenditureLedgerController : ControllerBase
    {
        private readonly IExpensesService expensesService;
        private readonly ITransactionCategoryService transactionCategoryService;
        private readonly ITransactionTypeService transactionTypeService;
        private readonly IImportDataService importDataService;
        private readonly IExpenditurePredictor expenditurePredictor;
        public ExpenditureLedgerController(IExpensesService _expensesService,
            ITransactionCategoryService _transactionCategoryService,
            ITransactionTypeService _transactionTypeService,
            IImportDataService _importDataService,
            IExpenditurePredictor _expenditurePredictor)
        {
            expensesService = _expensesService;
            transactionCategoryService = _transactionCategoryService;
            transactionTypeService = _transactionTypeService;
            importDataService = _importDataService;
            expenditurePredictor = _expenditurePredictor;
        }

        // GET: api/ExpenditureLedger/GetAllTransactionTypes
        [HttpGet("GetAllTransactionTypes")]
        public async Task<IEnumerable<TransactionTypeDto>> GetAllTransactionTypes()
        {
            var result = await transactionTypeService.GetAll();
            return result;
        }

        // GET: api/ExpenditureLedger/GetAllExpensesOfUser
        [HttpGet("GetAllExpensesOfUser")]
        public async Task<IEnumerable<ExpenseDto>> GetAllExpensesOfUser(int userId)
        {
            var result = await expensesService.GetAllByUserId(userId);
            return result;
        }

        // GET: api/ExpenditureLedger/GetRecentExpensesOfUser
        [HttpGet("GetRecentExpensesOfUser")]
        public async Task<IEnumerable<ExpenseDto>> GetRecentExpensesOfUser(int userId)
        {
            var result = await expensesService.GetAllByUserId(userId);
            return result.OrderByDescending(ex => ex.TransactionDate).Take(6);
        }

        // GET: api/ExpenditureLedger/GetExpensesOfUserByFilter
        [HttpGet("GetExpensesOfUserByFilter")]
        public async Task<IEnumerable<ExpenseDto>> GetExpensesOfUserByFilter([FromQuery] TransactionsFilterDto filter)
        {
            var result = await expensesService.GetAllByFilter(filter.User_Id, filter.Month, filter.Year);
            return result;
        }

        // GET: api/ExpenditureLedger/GetExpensePredictionForNextMonth
        [HttpGet("GetExpensePredictionForNextMonth")]
        public async Task<ExpensePredictorResult> GetExpensePredictionForNextMonth(int userId, int month, int year)
        {
            var result = await expenditurePredictor.PredictExpenses(userId, month, year);
            return result;
        }

        //POST: api/ExpenditureLedger/AddLedgerEntry
        [HttpPost("AddLedgerEntry")]
        public async Task<ExpenseDto> AddLedgerEntry([FromBody] ExpenseDto expenseDto)
        {
            var result = await expensesService.CreateNewLedgerEntry(expenseDto);
            return result;
        }

        //Delete: api/ExpenditureLedger/DeleteTransaction
        [HttpDelete("DeleteTransaction")]
        public async Task DeleteTransaction(int transactionId)
        {
            await expensesService.RemoveTransaction(transactionId);
        }

        //GET: api/ExpenditureLedger/GetAllCategoriesOfUser
        [HttpGet("GetAllCategoriesOfUser")]
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesOfUser(int userId)
        {
            var result = await transactionCategoryService.GetAllByUserId(userId);
            return result;
        }

        //POST: api/ExpenditureLedger/AddCategory
        [HttpPost("AddCategory")]
        public async Task<CategoryDto> AddCategory([FromBody] CategoryDto categoryDto)
        {
            var result = await transactionCategoryService.AddNewCategory(categoryDto);
            return result;
        }

        //Delete: api/ExpenditureLedger/DeleteCategory
        [HttpDelete("DeleteCategory")]
        public async Task DeleteCategory(int categoryId)
        {
            await transactionCategoryService.RemoveCategory(categoryId);
        }

        //POST: api/ExpenditureLedger/ImportDataFromCSV
        [HttpPost("ImportDataFromCSV")]
        public async Task<bool> ImportDataFromCSV(IFormFile importFile, [FromForm] int userId)
        {
            var result = await importDataService.ImportDataAsync(importFile, userId);
            return result;
        }

        //POST: api/ExpenditureLedger/ExtractExpenseDetailsFromBill
        [HttpPost("ExtractExpenseDetailsFromBill")]
        public async Task<BillDetailsExtractor> ExtractExpenseDetailsFromBill(IFormFile importFile, [FromForm] int userId)
        {
            var result = await importDataService.ExtractBillDataAsync(importFile, userId);
            return result;
        }
    }
}
