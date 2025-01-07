using Microsoft.AspNetCore.Identity;
using UserAuthApi.Data;
using UserAuthApi.Permissions;
using System.Linq;
using System.Threading.Tasks;
using UserAuthApi.Services;

namespace UserAuthApi.Models
{
    public class Seeder
    {
        public static void SeedPermissions(AppDbContext context)
        {
            try
            {
                if (!context.Permissions.Any())
                {
                    var permissions = new[]
                    {
                    new Permission { Name = "ReadApi" },
                    new Permission { Name = "WriteApi" },
                    new Permission { Name = "AdminPermission" }
                };

                    context.Permissions.AddRange(permissions);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding permissions: {ex.Message}");
            }
        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "user", "admin" };
            foreach (var roleName in roleNames)
            {
                if (roleManager.Roles.All(r => r.Name != roleName))
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        public static async Task SeedUsers(UserManager<User> userManager, AppDbContext context)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    Role = "admin"
                };
                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "admin");

                // Assign permissions
                var permissionService = new PermissionService(context);
                await permissionService.AssignPermissionToUser(user.Id, "AdminPermission");
                await permissionService.AssignPermissionToUser(user.Id, "ReadApi");
                await permissionService.AssignPermissionToUser(user.Id, "WriteApi");
            }
        }
    }

}
