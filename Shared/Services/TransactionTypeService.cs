using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface ITransactionTypeService
    {
        public Task<IEnumerable<TransactionTypeDto>> GetAll();
    }
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly ApplicationDbContext dbContext;

        public TransactionTypeService(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<IEnumerable<TransactionTypeDto>> GetAll()
        {
            List<TransactionTypeDto> transactionTypesDto = new List<TransactionTypeDto>();
            var transactionTypes = await dbContext.TransactionTypes.ToListAsync();
            
            foreach (var type in transactionTypes)
            {
                transactionTypesDto.Add(new TransactionTypeDto()
                {
                    Id = type.TT_Id,
                    TransactionType = type.TT_Name
                });
            }

            return transactionTypesDto;
        }
    }
}
