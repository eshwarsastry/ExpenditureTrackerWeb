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
        public Task AddBulkCategories(List<CategoryDto> categoryDtos);

        public Task RemoveCategory(int categoryId);

    }

    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserService userService;
        private readonly ICategoriesMapper categoriesMapper;
        
        public TransactionCategoryService(ApplicationDbContext _dbContext,
            IUserService _userService,
            ICategoriesMapper _categoriesMapper,
            ICategoriesMapper _categoriesMapperInstance)
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
                var transactionCategories = await dbContext.TransactionCategories
                    .Include(t => t.TC_TransactionType)
                    .Where(e => e.TC_User.U_Id == userId).ToListAsync();
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
                    if (categoryDto.Id != 0)
                    {
                        var existingCategory = await dbContext.TransactionCategories.Where(c => c.TC_Id == categoryDto.Id).FirstOrDefaultAsync();
                        if (existingCategory != null)
                        {
                            existingCategory.TC_Name = categoryDto.Name;
                            existingCategory.TC_Description = categoryDto.Description;
                            existingCategory.TC_TransactionType = transactionType;
                            existingCategory.TC_User = user;
                        }
                        dbContext.TransactionCategories.Update(existingCategory);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {

                        var category = new TransactionCategory()
                        {
                            TC_Name = categoryDto.Name,
                            TC_Description = categoryDto.Description,
                            TC_TransactionType = transactionType,
                            TC_User = user
                        };
                        dbContext.TransactionCategories.Add(category);
                        await dbContext.SaveChangesAsync();
                        categoryDto.Id = category.TC_Id;
                    }
                }

            }
            return categoryDto;
        }

        public async Task AddBulkCategories(List<CategoryDto> categoryDtos)
        {
            List<TransactionCategory> transactioncategoryEntities = new List<TransactionCategory>();
            // Map each CategoryDto to a TransactionCategory entity and add it to the database
            foreach (var categoryDto in categoryDtos)
            {
                var user = await dbContext.Users.Where(u => u.U_Id == categoryDto.User_Id).FirstOrDefaultAsync();
                if (user != null)
                {
                    var transactionType = await dbContext.TransactionTypes.Where(t => t.TT_Id == categoryDto.TransactionType_Id).FirstOrDefaultAsync();
                    if (transactionType != null)
                    {
                        var transactioncategoryEntity = categoriesMapper.ToTransactionCategoryEntity(categoryDto, transactionType, user);
                        transactioncategoryEntities.Add(transactioncategoryEntity);
                    }
                }
            }
            
            await dbContext.TransactionCategories.AddRangeAsync(transactioncategoryEntities);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveCategory(int categoryId)
        {
            var existingCategory = await dbContext.TransactionCategories.FindAsync(categoryId);
            if (existingCategory != null)
            {
                dbContext.TransactionCategories.Remove(existingCategory);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}

