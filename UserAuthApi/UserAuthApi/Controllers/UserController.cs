using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet("data")]
        [Authorize(Roles = "UserOnly")]
        public IActionResult GetUserData()
        {
            return Ok(new { message = "User data" });
        }
    }
}





