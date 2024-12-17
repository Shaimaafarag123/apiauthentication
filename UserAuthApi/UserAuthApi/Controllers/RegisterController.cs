using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserAuthApi.Models;
using UserAuthApi.dto;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterController> _logger;
        public RegisterController(UserManager<User> userManager, ILogger<RegisterController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already exists." });
            }
            //Create New User
            var newUser = new User
            {
                UserName = request.Username,
                Role = request.Role
            };
            //Save New User
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
            {
                //user creation fails, logs the errors and returns a 400 Bad Request with error details.
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
                return BadRequest(new { message = "User registration failed.", errors = result.Errors });
            }
            if (!string.IsNullOrEmpty(request.Role))
            {
                var roleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Error assigning role: " + string.Join(", ", roleResult.Errors));
                }
            }
            return CreatedAtAction(nameof(Register), new { username = newUser.UserName }, new { message = "User registered successfully." });
        }
    }
}