using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("data")] 
        //[Authorize(Policy = "AdminOnly")] 
        public IActionResult GetAdminData()

        {

            return Ok(new { message = "Admin data" });
        }
    }
}

