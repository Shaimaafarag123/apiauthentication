using Microsoft.AspNetCore.Identity;
using UserAuthApi.Data;

namespace UserAuthApi.Models
{
    public class Seeder
    {
        //public static void SeedPermissions(AppDbContext context)
        //{
        //    if (!context.Permissions.Any())
        //    {
        //        var permissions = new List<Permission>
        //    {
        //        new Permission { Name = "ViewProfile" },
        //        new Permission { Name = "EditProfile" },
        //        new Permission { Name = "DeleteAccount" }
        //    };
        //        context.Permissions.AddRange(permissions);
        //        context.SaveChanges();
        //    }
        //} 
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "user", "User", "admin", "Admin" };
            foreach (var roleName in roleNames)
            {
                if (roleManager.Roles.All(r => r.Name != roleName))
                {

                    var role = new IdentityRole(roleName);
                    roleManager.CreateAsync(role).Wait();
                }
            }
        }
    }

}