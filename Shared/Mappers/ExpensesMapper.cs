using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Dto.Agent;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Services;

namespace ExpenditureTrackerWeb.Shared.Mappers
{
    public interface IExpensesMapper
    {
        public ExpenseDto ToExpenseDto(Expense expense);
        public ExpenseDto ToExpenseDto(BillDetailsAnalyserDto billDetails, CategoryDto catgeoryDto, UserDto userDto);
        public Expense ToExpenseEntity(ExpenseDto expenseDto, TransactionCategory transactionCategory, User user);
    }

    public class ExpensesMapper : IExpensesMapper
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

        public ExpenseDto ToExpenseDto(BillDetailsAnalyserDto billDetails, CategoryDto catgeoryDto, UserDto userDto)
        {
            return new ExpenseDto()
            {
                User_Id = userDto.Id,
                Category_Id = catgeoryDto.Id,
                Category_Name = catgeoryDto.Name,
                TransactionType_Id = catgeoryDto.TransactionType_Id,
                TransactionType_Name = catgeoryDto.TransactionType_Name,
                Amount = (decimal)billDetails.BillAmount,
                TransactionDate = billDetails.BillDate,
                Note = billDetails.Note,
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
