using Microsoft.AspNetCore.Identity;

namespace UserAuthApi.Models
{
    public class Seeder
    {
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] {"user","User", "admin" ,"Admin" }; 
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
