using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GitLab.Majordome.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpPost("refresh-keyboard")]
        public async Task<IActionResult> RefreshKeyboardAsync()
        {
            await adminService.RefreshKeyboardAsync();

            return NoContent();
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddUserAsync([FromBody] User user)
        {
            await adminService.AddUserAsync(user);

            return NoContent();
        }
    }
}