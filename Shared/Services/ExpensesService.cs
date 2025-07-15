using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Dto.Predictor;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface IExpensesService
    {
        public Task<IEnumerable<ExpenseDto>> GetAllByUserId(int userId);
        public Task<ExpenseDto> CreateNewLedgerEntry(ExpenseDto expenseDto);
        public Task RemoveTransaction(int transactionId);
        public Task<IEnumerable<ExpenseDto>> GetAllByFilter(int userId, int month, int year);
        public Task AddBulkExpenses(List<ExpenseDto> expenseDtos);
        public Task<List<ExpensePredictorData>> GetExpensesOfUserforPrediction(int userId);

    }
    public class ExpensesService : IExpensesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IExpensesMapper expensesMapper;
        private readonly IUserService userService;

        public ExpensesService(ApplicationDbContext _dbContext,
            IExpensesMapper _expensesMapper,
            IUserService _userService)
        {
            dbContext = _dbContext;
            expensesMapper = _expensesMapper;
            userService = _userService;
        }

        public async Task<IEnumerable<ExpenseDto>> GetAllByUserId(int userId)
        {
            var userDto = await userService.GetUserEntity(userId);
            List<ExpenseDto> expensesDto = new List<ExpenseDto>();
            if (userDto != null)
            {
                var expenses = await dbContext.Expenses
                    .Include(tc => tc.EX_TransactionCategory)
                    .Include(t => t.EX_TransactionCategory.TC_TransactionType)
                    .Where(e => e.EX_User.U_Id == userId).ToListAsync();
                foreach (var expense in expenses)
                {
                    var expenseDto = expensesMapper.ToExpenseDto(expense);
                    expensesDto.Add(expenseDto);
                }
            }
            return expensesDto;
        }

        public async Task<IEnumerable<ExpenseDto>> GetAllByFilter(int userId, int month, int year)
        {
            var userDto = await userService.GetUserEntity(userId);
            List<ExpenseDto> expensesDto = new List<ExpenseDto>();
            if (userDto != null)
            {
                var expenses = await dbContext.Expenses
                    .Include(tc => tc.EX_TransactionCategory)
                    .Include(t => t.EX_TransactionCategory.TC_TransactionType)
                    .Where(e => e.EX_User.U_Id == userId && e.EX_DateTime.Month == month && e.EX_DateTime.Year == year).ToListAsync();
                foreach (var expense in expenses)
                {
                    var expenseDto = expensesMapper.ToExpenseDto(expense);
                    expensesDto.Add(expenseDto);
                }
            }
            return expensesDto;
        }

        public async Task<ExpenseDto> CreateNewLedgerEntry(ExpenseDto expenseDto)
        {
            var user = await dbContext.Users.Where(u => u.U_Id == expenseDto.User_Id).FirstOrDefaultAsync();
            if (user != null)
            {
                var transactionCategory = await dbContext.TransactionCategories.Where(t => t.TC_Id == expenseDto.Category_Id).FirstOrDefaultAsync();
                if (transactionCategory != null)
                {
                    if (expenseDto.Id != 0)
                    {
                        var existingExpense = await dbContext.Expenses.FindAsync(expenseDto.Id);
                        if (existingExpense != null)
                        {
                            existingExpense.EX_Amount = expenseDto.Amount;
                            existingExpense.EX_DateTime = expenseDto.TransactionDate;
                            existingExpense.EX_Note = expenseDto.Note;
                            existingExpense.EX_User = user;
                            existingExpense.EX_TransactionCategory = transactionCategory;
                            dbContext.Update(existingExpense);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var expense = new Expense()
                        {
                            EX_Amount = expenseDto.Amount,
                            EX_DateTime = expenseDto.TransactionDate,
                            EX_Note = expenseDto.Note,
                            EX_User = user,
                            EX_TransactionCategory = transactionCategory
                        };
                        dbContext.Add(expense);
                        await dbContext.SaveChangesAsync();

                        expenseDto.Id = expense.EX_Id;

                    }
                }
            }
            return expenseDto;
        }

        public async Task AddBulkExpenses(List<ExpenseDto> expenseDtos)
        {
            List<Expense> expenseEntities = new List<Expense>();
            // Map each ExpenseDto to a Expense entity and add it to the database
            foreach (var expenseDto in expenseDtos)
            {
                var user = await dbContext.Users.Where(u => u.U_Id == expenseDto.User_Id).FirstOrDefaultAsync();
                if (user != null)
                {
                    var transactionCategory = await dbContext.TransactionCategories.Where(t => t.TC_Name == expenseDto.Category_Name).FirstOrDefaultAsync();
                    if (transactionCategory != null)
                    {
                        var expenseEntity = expensesMapper.ToExpenseEntity(expenseDto, transactionCategory, user);
                        expenseEntities.Add(expenseEntity);
                    }
                }
            }

            await dbContext.Expenses.AddRangeAsync(expenseEntities);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<ExpensePredictorData>> GetExpensesOfUserforPrediction(int userId)
        {
            // Group all the expenses by month, year and sum the amounts and store the result in expensePredictorDatas.
            var groupedExpenses = await dbContext.Expenses
                .Where(expense => expense.EX_User.U_Id == userId && expense.EX_Amount < 0 
                && expense.EX_DateTime.Month != DateTime.Now.Month) // Exclude current year and month and take only expenses
                .GroupBy(g => new { g.EX_UserU_Id, g.EX_DateTime.Year, g.EX_DateTime.Month })
                .Select(e => new ExpensePredictorData
                {
                    UserId = e.Key.EX_UserU_Id,
                    Month = e.Key.Month,
                    Year = e.Key.Year,
                    Amount = (float)e.Sum(g => g.EX_Amount) // Explicit cast to float
                })
                .ToListAsync();
            
            return groupedExpenses;
        }

        public async Task RemoveTransaction(int transactionId)
        {
            var existingTransaction = await dbContext.Expenses.FindAsync(transactionId);
            if (existingTransaction != null)
            {
                dbContext.Expenses.Remove(existingTransaction);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
