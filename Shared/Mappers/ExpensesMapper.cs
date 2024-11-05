using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;

namespace ExpenditureTrackerWeb.Shared.Mappers
{
    public interface IExpensesMapper
    {
        public ExpenseDto ToExpenseDto(Expense expense);
    }

    public class ExpensesMapper: IExpensesMapper
    {
        public ExpenseDto ToExpenseDto(Expense expense)
        {
            return new ExpenseDto()
            {
                Id = expense.EX_Id,
                User_Id = expense.EX_User.U_Id,
                Category_Id = expense.EX_TransactionCategory.TC_Id,
                Amount = expense.EX_Amount,
                TransactionDate = expense.EX_DateTime,
                Note = expense.EX_Note,
            };

        }
    }
}
