using Microsoft.AspNetCore.Identity;
using PermissionBasedAuthorizationIntDotNet5.Contants;

namespace UserAuthApi.Seeds
{
    public static class DefaultRoles
    {
        public static async Task Seedasync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
            }
        }
    }
}