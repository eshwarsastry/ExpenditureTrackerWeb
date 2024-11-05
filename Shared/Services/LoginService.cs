using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenditureTrackerWeb.Shared.Services
{
    public interface ILoginService
    {
        public Task<bool> Login(string email, string password);

    }

    public class LoginService : ILoginService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAuthService authService;
        private readonly IUserService userService;
        public LoginService(
            ApplicationDbContext _dbContext,
            IAuthService _authService,
            IUserService _userService) 
        {
            dbContext = _dbContext;
            authService = _authService;
            userService = _userService;
        }

        public async Task<bool> Login(string email, string password)
        {
            var isUserValid = await dbContext.Users.Where(u => u.U_Email == email &&
            u.U_Password == authService.ComputeHash(password)).AnyAsync();
            return isUserValid;
        }
    }
}
