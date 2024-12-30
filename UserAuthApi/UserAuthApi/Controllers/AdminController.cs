using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        // Assign a single permission to a user by username
        [HttpPost("assign-permission/{username}")]
        public async Task<IActionResult> AssignPermission([FromRoute] string username, [FromBody] string permission)
        {
            try
            {
                // Validate the permission before assigning
                var allPermissions = await _permissionService.GetAllPermissions();
                if (!allPermissions.Any(p => p.Name == permission))
                {
                    return BadRequest(new { message = $"Invalid permission '{permission}'." });
                }

                await _permissionService.AssignPermissionToUserByUsername(username, permission);
                return Ok(new { message = $"Permission '{permission}' assigned to user {username}." });
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (PermissionNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        // Assign multiple permissions to a user by username
        [HttpPost("assign-permissions/{username}")]
        public async Task<IActionResult> AssignPermissions([FromRoute] string username, [FromBody] List<string> permissions)
        {
            try
            {
                // Validate all permissions before assigning
                var allPermissions = await _permissionService.GetAllPermissions();
                foreach (var permission in permissions)
                {
                    if (!allPermissions.Any(p => p.Name == permission))
                    {
                        return BadRequest(new { message = $"Invalid permission '{permission}'." });
                    }
                }

                await _permissionService.AssignPermissionsToUser(username, permissions);
                return Ok(new { message = $"Permissions assigned to user {username}." });
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (PermissionNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Admin test route" });
        }
    }
}
