using Microsoft.AspNetCore.Identity;
using PermissionBasedAuthorizationIntDotNet5.Contants;
using System.Security.Claims;
using UserAuthApi.Constant;

namespace UserAuthApi.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedBaicUserAsync(UserManager<User> userManager)
        {
            var defaultUser = new User
            {
                UserName = "Basicuser@domain.com",
                Email = "Basicuser@domain.com",
                EmailConfirmed = true,
            };

            var user =  userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(defaultUser, "P@ssword123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                }
                else
                {
                    // Log errors if user creation fails
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
        }

        public static async Task SeedSuperadminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new User
            {
                UserName = "Superadmin@domain.com",
                Email = "Superadmin@domain.com",
                EmailConfirmed = true,
            };

            var user =  userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(defaultUser, "P@ssword123");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(defaultUser, new List<string>
            {
                Roles.Basic.ToString(),
                Roles.Admin.ToString(),
                Roles.SuperAdmin.ToString()
            });
                }
                else
                {
                    // Log errors if user creation fails
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }

            await roleManager.SeedClaimsForSuperUser();
        }



        public static async Task SeedClaimsForSuperUser(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(adminRole, "Products");
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);

            var allPermissions = Permissions.GeneratePermissionList(module);

            foreach (var permission in allPermissions)
            {
                if (allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }






    }
}