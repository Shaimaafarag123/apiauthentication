﻿using Microsoft.EntityFrameworkCore;
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
            await AssignPermissionsToUser(userId, new[] { permission });
        }

        // Assign a permission to a user by their username
        public async Task AssignPermissionToUserByUsername(string username, string permission)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new UserNotFoundException(username);
            }

            var existingPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == user.Id && up.Permission.Name == permission);

            if (existingPermission)
            {
                throw new PermissionAlreadyAssignedException($"Permission '{permission}' is already assigned to user '{username}'.");
            }

            await AssignPermissionsToUser(user.Id, new[] { permission });
        }


        // Batch permission assignment to a user
        public async Task AssignPermissionsToUser(string userId, IEnumerable<string> permissions)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var permissionsToAssign = await _context.Permissions
                .Where(p => permissions.Contains(p.Name))
                .ToListAsync();

            // Check if all permissions are valid
            if (permissionsToAssign.Count != permissions.Distinct().Count())
            {
                throw new PermissionNotFoundException("Some permissions are invalid.");
            }

            // Get already assigned permissions
            var existingPermissionIds = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Filter out already assigned permissions
            var newPermissions = permissionsToAssign
                .Where(p => !existingPermissionIds.Contains(p.Id))
                .Select(p => new UserPermission
                {
                    UserId = userId,
                    PermissionId = p.Id
                })
                .ToList();

            if (!newPermissions.Any())
            {
                throw new PermissionAlreadyAssignedException("All permissions are already assigned.");
            }

            await _context.UserPermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }


        // Retrieve all permissions from the database
        public async Task<IEnumerable<Permission>> GetAllPermissions()
        {
            return await _context.Permissions.ToListAsync();
        }
    }

    // Custom exceptions for user and permission not found scenarios
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string username) : base($"User '{username}' not found.") { }
    }

    public class PermissionNotFoundException : Exception
    {
        public PermissionNotFoundException(string permission) : base($"Permission '{permission}' not found.") { }
    }

    public class PermissionAlreadyAssignedException : Exception
    {
        public PermissionAlreadyAssignedException(string message) : base(message) { }
    }
}
