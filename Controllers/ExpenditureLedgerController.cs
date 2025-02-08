using ExpenditureTrackerWeb.Shared.Dto;
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
        private readonly IUserService userService;
        private readonly ITransactionCategoryService transactionCategoryService;
        private readonly ITransactionTypeService transactionTypeService;

        public ExpenditureLedgerController(IExpensesService _expensesService,
            IUserService _userService,
            ITransactionCategoryService _transactionCategoryService,
            ITransactionTypeService _transactionTypeService)
        {
            expensesService = _expensesService;
            userService = _userService;
            transactionCategoryService = _transactionCategoryService;
            transactionTypeService = _transactionTypeService;
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

        // GET: api/ExpenditureLedger/GetExpensesOfUserByFilter
        [HttpGet("GetExpensesOfUserByFilter")]
        public async Task<IEnumerable<ExpenseDto>> GetExpensesOfUserByFilter([FromQuery] TransactionsFilterDto filter)
        {
            var result = await expensesService.GetAllByFilter(filter.User_Id, filter.Month, filter.Year);
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
    }
}
