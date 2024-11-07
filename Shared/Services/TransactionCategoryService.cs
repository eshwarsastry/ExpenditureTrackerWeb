using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface ITransactionCategoryService
    {
        public Task<IEnumerable<CategoryDto>> GetAllByUserId(int userId);
        public Task<CategoryDto> AddNewCategory(CategoryDto categoryDto);
    }

    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserService userService;
        private readonly ICategoriesMapper categoriesMapper;

        public TransactionCategoryService(ApplicationDbContext _dbContext,
            IUserService _userService,
            ICategoriesMapper _categoriesMapper)
        {
            dbContext = _dbContext;
            userService = _userService;
            categoriesMapper = _categoriesMapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllByUserId(int userId)
        {
            var userDto = await userService.GetUserEntity(userId);
            List<CategoryDto> transactionCategoriesDto = new List<CategoryDto>();

            if (userDto != null)
            {
                var transactionCategories = await dbContext.TransactionCategories.Where(e => e.TC_User.U_Id == userId).ToListAsync();
                foreach (var category in transactionCategories)
                {
                    var expenseDto = categoriesMapper.ToCategoryDto(category);
                    transactionCategoriesDto.Add(expenseDto);
                }
            }
            return transactionCategoriesDto;
        }

        public async Task<CategoryDto> AddNewCategory(CategoryDto categoryDto)
        {
            var user = await dbContext.Users.Where(u => u.U_Id == categoryDto.User_Id).FirstOrDefaultAsync();
            if (user != null)
            {
                var transactionType = await dbContext.TransactionTypes.Where(t => t.TT_Id == categoryDto.TransactionType_Id).FirstOrDefaultAsync();
                if (transactionType != null)
                {
                    var category = new TransactionCategory()
                    {
                        TC_Name = categoryDto.Name,
                        TC_Description = categoryDto.Description,
                        TC_TransactionType = transactionType,
                        TC_User = user
                    };
                    dbContext.Add(category);
                    await dbContext.SaveChangesAsync();

                    categoryDto.Id = category.TC_Id;
                }

            }
            return categoryDto;
        }
    }
}

