using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace BookCRUD.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET: api/values
        [HttpGet]
        [Authorize] // Requires authentication
        public IActionResult GetValues()
        {
            // Access user claims if needed
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from token
            var userName = User.FindFirstValue(ClaimTypes.Name); // Get username from token
            var userRole = User.FindFirstValue(ClaimTypes.Role); // Get role from token

            return Ok(new List<string> {
                "value1 from protected endpoint",
                "value2 from protected endpoint",
                $"User ID: {userId}",
                $"Username: {userName}",
                $"Role: {userRole}"
            });
        }

        // GET: api/values/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // Requires authentication and "Admin" role
        public IActionResult GetAdminValues()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new List<string> {
                "admin_value1 from role-protected endpoint",
                "admin_value2 from role-protected endpoint",
                $"Welcome Admin: {userName}"
            });
        }

        // GET: api/values/public
        [HttpGet("public")]
        [AllowAnonymous] // Allows access without authentication
        public IActionResult GetPublicValues()
        {
            return Ok(new List<string> { "public_value1", "public_value2" });
        }
    }
}
