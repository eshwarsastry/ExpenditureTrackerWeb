using Microsoft.AspNetCore.Mvc;
using ExpenditureTrackerWeb.Shared.Models;
using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Services;
using Microsoft.AspNetCore.Cors;
using ExpenditureTrackerWeb.Shared.ResponseModels;
using NuGet.Protocol.Plugins;
using ExpenditureTrackerWeb.Shared.Enums;

namespace ExpenditureTrackerWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILoginService loginService;
        public LoginController(IUserService _userService,
            ILoginService _loginService)
        {
            userService = _userService;
            loginService = _loginService;
        }

        // GET: api/Login/GetAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await userService.GetUserEntities();
        }

        // POST: api/Login/LoginUser
        [HttpPost("LoginUser")]
        public async Task<LoginResponse> LoginUser([FromBody] UserDto userDto)
        {
            var isUserPresent = await userService.UserExists(userDto);
            var loginResponse = new LoginResponse();

            if (isUserPresent)
            {
                var isLoginValid = await loginService.Login(userDto.Email, userDto.Password);
                loginResponse.Username = userDto.Email;

                if (isLoginValid)
                {
                    loginResponse.UserId = await userService.GetUserIdByUserEmailEntity(userDto);
                    loginResponse.ResponseCode = (int)ResponseEnums.Login_Success;
                }
                else
                {
                    loginResponse.ResponseCode = (int)ResponseEnums.Username_Password_Mismatch;
                }
            }
            else
            {
                loginResponse.ResponseCode = (int)ResponseEnums.Username_Not_Found;
            }

            return loginResponse;
        }

        // POST: api/Login/CreateUser
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateUser")]
        public async Task<RegisterResponse> CreateUser([FromBody] UserDto userDto)
        {
            var isUserPresent = await userService.UserExists(userDto);
            RegisterResponse registerResponse = new RegisterResponse();

            if (!isUserPresent)
            {
                userDto = await userService.CreateUserAsync(userDto);
                registerResponse.ResponseCode = (int)ResponseEnums.User_Created;
            }
            else
            {
                registerResponse.ResponseCode = (int)ResponseEnums.User_Already_Exists;
            }
            return registerResponse;
        }
    }
}
