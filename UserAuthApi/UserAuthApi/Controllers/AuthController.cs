using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserAuthApi.Models;
using UserAuthApi.Services;
using Microsoft.AspNetCore.Identity;  

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;  

        public AuthController(IAuthService authService, IUserService userService, UserManager<User> userManager)
        {
            _authService = authService;
            _userService = userService;
            _userManager = userManager;  
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginUser.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var authResponse = await _authService.AuthenticateAsync(loginUser.Username, loginUser.Password);

            return Ok(authResponse);  
        }
    }
}
