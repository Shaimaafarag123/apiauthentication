using UserAuthApi.Data;
using UserAuthApi.Permissions;
using Microsoft.EntityFrameworkCore;

namespace UserAuthApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        // Change Guid to string to match the interface
        public async Task<IEnumerable<string>> GetPermissionsForUser(string userId)
        {
            return await _context.UserPermissions
                .Where(up => up.UserId == userId)  // Use UserId instead of Id
                .Select(up => up.Permission.Name)  // Extracting the Permission Name as a string
                .ToListAsync();
        }

        public async Task AssignPermissionToUser(string userId, string permission)
        {
            // Fetch the Permission object from the database based on the permission string
            var permissionObj = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            // If the permission doesn't exist, you could throw an exception or handle it in another way
            if (permissionObj == null)
            {
                throw new Exception($"Permission '{permission}' not found.");
            }

            var userPermission = new UserPermission
            {
                UserId = userId,  // Store the UserId (string) as a foreign key
                PermissionId = permissionObj.Id,  // Store the PermissionId
                Permission = permissionObj  // Optional: if you want to keep the Permission object
            };

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }
    }
}
