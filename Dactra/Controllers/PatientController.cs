using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;
        public PatientController(IPatientService patientService , UserManager<ApplicationUser> userManager)
        {
            _patientService = patientService;
            _userManager = userManager;
        }
        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(PatientCompleteDTO patientComplateDTO)
        {
            await _patientService.CompleteRegistrationAsync(patientComplateDTO);
            return Ok();
        }

        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var PatientProfiles = await _patientService.GetAllProfileAsync();
            return Ok(PatientProfiles);
        }
        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid token" });
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized(new { message = "User not found" });
            var profile = await _patientService.GetProfileByUserID(user.Id);
            return Ok(profile);
        }

        [HttpDelete("DeletePatient/{patientId}")]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            await _patientService.DeletePatientProfileAsync(patientId);
            return Ok("Profile Deleted Succesfully");
        }
    }
}
