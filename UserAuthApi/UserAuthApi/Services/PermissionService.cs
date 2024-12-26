using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthApi.Data;
using UserAuthApi.Permissions;

namespace UserAuthApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve all permissions assigned to a specific user by user ID
        public async Task<IEnumerable<string>> GetPermissionsForUser(string userId)
        {
            return await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permission.Name)
                .ToListAsync();
        }

        // Assign a permission to a user by their user ID
        public async Task AssignPermissionToUser(string userId, string permission)
        {
            var permissionObj = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permission);

            if (permissionObj == null)
            {
                throw new Exception($"Permission '{permission}' not found.");
            }

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionObj.Id
            };

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        // Assign a permission to a user by their username
        public async Task AssignPermissionToUserByUsername(string username, string permission)
        {
            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                throw new Exception($"User '{username}' not found.");
            }

            // Fetch the Permission object from the database
            var permissionObj = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == permission);

            if (permissionObj == null)
            {
                throw new Exception($"Permission '{permission}' not found.");
            }

            // Create the UserPermission entity
            var userPermission = new UserPermission
            {
                UserId = user.Id,
                PermissionId = permissionObj.Id
            };

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        // Retrieve all permissions from the database
        public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            return await _context.Permissions.ToListAsync();
        }
    }
}
