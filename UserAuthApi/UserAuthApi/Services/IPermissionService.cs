namespace UserAuthApi.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<string>> GetPermissionsForUser(string userId);
        Task AssignPermissionToUser(string userId, string permission);
        Task AssignPermissionToUserByUsername(string username, string permission); // New Method
    }

}
