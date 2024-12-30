using UserAuthApi.Permissions;

namespace UserAuthApi.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<string>> GetPermissionsForUser(string userId);
        Task AssignPermissionToUser(string userId, string permission);
        Task AssignPermissionToUserByUsername(string username, string permission);
        Task AssignPermissionsToUser(string userId, IEnumerable<string> permissions); 
        Task<IEnumerable<Permission>> GetAllPermissions(); 
    }
}
