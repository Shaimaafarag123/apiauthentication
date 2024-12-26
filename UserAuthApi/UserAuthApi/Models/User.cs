using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UserAuthApi.Models;
using UserAuthApi.Permissions;

public class User : IdentityUser
{
    public string Role { get; set; }
    public ICollection<UserPermission> UserPermissions { get; set; }

} 