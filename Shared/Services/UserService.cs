using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUserEntities();
        public Task<UserDto> CreateUserAsync(UserDto userDto);
        public Task<Boolean> UserExists(UserDto userDto);
        public Task<UserDto> GetUserEntity(int userId);
        public Task<int> GetUserIdByUserEmailEntity(UserDto userDto);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAuthService authService;
        private readonly IUsersMapper usersMapper;
        public UserService(ApplicationDbContext _dbContext,
            IAuthService _authService,
            IUsersMapper _usersMapper)
        {
            dbContext = _dbContext;
            authService = _authService;
            usersMapper = _usersMapper;
        }

        public async Task<IEnumerable<User>> GetUserEntities()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<UserDto> GetUserEntity(int userId)
        {
            var user = await dbContext.Users.Where(u => u.U_Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return new UserDto();
            }
            else
            {
                var userDto = usersMapper.ToUserDto(user);
                return userDto;
            }
        }

        public async Task<int> GetUserIdByUserEmailEntity(UserDto userDto)
        {
            var user = await dbContext.Users.Where(u => u.U_Email == userDto.Email).FirstOrDefaultAsync();
            return user.U_Id;
        }

        public async Task<Boolean> UserExists(UserDto userDto)
        {
            if (userDto != null && userDto.Email != String.Empty)
            {
                var user = await dbContext.Users.Where(u => u.U_Email == userDto.Email).FirstOrDefaultAsync();
                return (user != null);
            }
            return false;
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {

            User user = new User()
            {
                U_FirstName = userDto.FirstName,
                U_Name = userDto.LastName,
                U_Email = userDto.Email,
                U_Password = authService.ComputeHash(userDto.Password),
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            userDto.Id = user.U_Id;

            return userDto;
        }
    }
}
