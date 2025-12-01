using Dactra.DTOs.Admin;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IAdminService adminService, UserManager<ApplicationUser> userManager)
        {
            _service = adminService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("Seed")]
        public async Task<IActionResult> Seed()
        {
            return Ok(await _service.SeedAdmin());
        }

       
        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin(CreateAdminDto dto)
        {
            return Ok(await _service.AddAdmin(dto));
        }

      
        [HttpGet("All")]
        public async Task<IActionResult> GetAdmins()
        {
            return Ok(await _service.GetAdmins());
        }

        
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _service.DeleteAdmin(id));
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _service.GetById(id);
            if (user == null) return NotFound("Admin not found");
            return Ok(user);
        }

        [HttpGet("getByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _service.GetByEmail(email);
            if (user == null) return NotFound("Admin not found");
            return Ok(user);
        }
        [HttpPost("makeAdmin/{id}")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            await _service.AddToAdminRole(user);
            return Ok("User is now Admin");
        }
    }
}
