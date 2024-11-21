using System.Threading.Tasks;
using UserAuthApi.Models;

namespace UserAuthApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(string username, string password);
    }
}
