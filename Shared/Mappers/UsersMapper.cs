using ExpenditureTrackerWeb.Shared.Dto;
using ExpenditureTrackerWeb.Shared.Entities;
using ExpenditureTrackerWeb.Shared.Models;

namespace ExpenditureTrackerWeb.Shared.Mappers
{
    public interface IUsersMapper
    {
        public UserDto ToUserDto(User user);
    }

    public class UsersMapper: IUsersMapper
    {
        public UserDto ToUserDto(User user)
        {
            return new UserDto()
            {
                Id = user.U_Id,
                FirstName = user.U_FirstName,
                LastName = user.U_Name,
                Email = user.U_Email,
                Password = user.U_Password
            };

        }
    }
}
