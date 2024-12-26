namespace UserAuthApi.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<string>> GetPermissionsForUser(string userId);  // Expecting string userId
        Task AssignPermissionToUser(string userId, string permission);
    }
}
