using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;

namespace ExpenditureTrackerWeb.Shared.Mappers
{
    public interface IExpensesMapper
    {
        public ExpenseDto ToExpenseDto(Expense expense);
        public Expense ToExpenseEntity(ExpenseDto expenseDto, TransactionCategory transactionCategory, User user);
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
                Category_Name = expense.EX_TransactionCategory.TC_Name,
                TransactionType_Id = expense.EX_TransactionCategory.TC_TransactionTypeTT_Id,
                TransactionType_Name = expense.EX_TransactionCategory.TC_TransactionType.TT_Name,
                Amount = expense.EX_Amount,
                TransactionDate = expense.EX_DateTime,
                Note = expense.EX_Note,
            };

        }

        public Expense ToExpenseEntity(ExpenseDto expenseDto, TransactionCategory transactionCategory, User user)
        {
            return new Expense()
            {
                EX_Id = expenseDto.Id,
                EX_User = user,
                EX_TransactionCategory = transactionCategory,
                EX_Amount = expenseDto.Amount,
                EX_DateTime = expenseDto.TransactionDate,
                EX_Note = expenseDto.Note
            };
        }
    }
}
