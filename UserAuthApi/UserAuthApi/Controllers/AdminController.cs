using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserAuthApi.Controllers
{
    // Controllers/AdminController.cs
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminPolicy")] 
    public class AdminController : ControllerBase
    {
        [HttpGet("data")]
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "Admin data" });
        }
    }

}

