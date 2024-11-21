using System.Threading.Tasks;
using UserAuthApi.Repositories;

namespace UserAuthApi.Data
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
