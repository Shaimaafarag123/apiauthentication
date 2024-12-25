// File: Services/CustomAuthorizationHandler.cs
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using UserAuthApi.Models;

public class CustomAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(c => c.Type == "role");

        Console.WriteLine($"Role Claim: {roleClaim?.Value}");

        if (roleClaim != null && roleClaim.Value == "Admin")
        {
            context.Succeed(requirement);
        }
        else
        {
            Console.WriteLine("Authorization failed: role is not Admin");
        }

        return Task.CompletedTask;
    }

}

