using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthApi.dto;
using UserAuthApi.Models;

namespace UserAuthApi.Services
{
    public interface IUserService
    {
        Task<RegisterUserDto> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user, string plainPassword);
    }
}
