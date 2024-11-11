using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;

namespace ExpenditureTrackerWeb.Shared.Mappers
{
    public interface ICategoriesMapper
    {
        public CategoryDto ToCategoryDto(TransactionCategory transactionCategory);
    }

    public class CategoriesMapper: ICategoriesMapper
    {
        public CategoryDto ToCategoryDto(TransactionCategory transactionCategory)
        {
            return new CategoryDto()
            {
                Id = transactionCategory.TC_Id,
                User_Id = transactionCategory.TC_UserU_Id,
                Name= transactionCategory.TC_Name,
                Description = transactionCategory.TC_Description,
                TransactionType_Id = transactionCategory.TC_TransactionTypeTT_Id,
                TransactionType_Name = transactionCategory.TC_TransactionType?.TT_Name
            };

        }
    }
}
