using System.Threading.Tasks;
using System.Collections.Generic;
using UserAuthApi.Models;

namespace UserAuthApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
