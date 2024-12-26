using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserAuthApi.Services;

namespace UserAuthApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin")]  
    public class AdminController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public AdminController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("data")]
        [Authorize(Policy = "AdminPermission")]  
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "Admin data accessible to authorized users with the right permissions." });
        }

        // Assign permission to a user by username
        [HttpPost("assign-permission/{username}")]
        public async Task<IActionResult> AssignPermission([FromRoute] string username, [FromBody] string permission)
        {
            try
            {
                await _permissionService.AssignPermissionToUserByUsername(username, permission);
                return Ok(new { message = $"Permission '{permission}' assigned to user {username}." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Admin test route" });
        }
    }
}
