using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuthorizationIntDotNet5.ViewModel;

namespace PermissionBasedAuthorizationIntDotNet5.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers()
        {
            var users = await _userManager.Users
                .Select(user => new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/manageRoles/{userId}
        [HttpGet("manageRoles/{userId}")]
        public async Task<ActionResult<UserRolesViewModel>> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new CheckBoxViewModel
                {
                    DisplayValue = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };

            return Ok(viewModel);
        }

        // POST: api/Users/updateRoles
        [HttpPost("updateRoles")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles([FromBody] UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            var userRoles = await _userManager.GetRolesAsync(user);

            // Remove existing roles
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            // Add selected roles
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.DisplayValue));

            return Ok(new { message = "User roles updated successfully." });
        }
    }
}
