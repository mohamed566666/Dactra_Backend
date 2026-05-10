using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;
using Dactra.Enums;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalTestsProviderController : ControllerBase
    {
        private readonly IMedicalTestsProviderService _medicalTestsProviderService;
        private readonly IPatientService _patientService;

        public MedicalTestsProviderController(IMedicalTestsProviderService medicalTestsProviderService, IPatientService patientService)
        {
            _medicalTestsProviderService = medicalTestsProviderService;
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patientId = await GetPatientIdAsync();
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetAllProfilesAsync(patientId);
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            try
            {
                var patientId = await GetPatientIdAsync();
                var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByIdAsync(Id, patientId);
                return Ok(MedicalTestProviderProfile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(MedicalTestsProviderUpdateDTO medicalTestProviderDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            try
            {
                await _medicalTestsProviderService.UpdateProfileAsync(userId, medicalTestProviderDTO);
                return Ok("Profile Updated Succesfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(MedicalTestProviderDTO medicalTestProviderDTO)
        {
            try
            {
                await _medicalTestsProviderService.CompleteRegistrationAsync(medicalTestProviderDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetByUserId/{UserId}")]
        public async Task<IActionResult> GetProfileByUserId(string UserId)
        {
            try
            {
                var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByUserIdAsync(UserId);
                return Ok(MedicalTestProviderProfile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetApproved")]
        public async Task<IActionResult> GetApproved()
        {
            var patientId = await GetPatientIdAsync();
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetApprovedProfilesAsync(null, patientId);
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpGet("GetByType/{type}")]
        public async Task<IActionResult> GetByType(MedicalTestProviderType type)
        {
            var patientId = await GetPatientIdAsync();
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetProfilesByTypeAsync(type, patientId);
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found");
            }

            try
            {
                var profile = await _medicalTestsProviderService.GetProfileByUserIdAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProfile(int Id)
        {
            try
            {
                await _medicalTestsProviderService.DeleteMedicalTestProviderProfileAsync(Id);
                return Ok("Profile Deleted Succesfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] MedicalTestProviderSearchFilterDTO filter)
        {
            var patientId = await GetPatientIdAsync();
            var result = await _medicalTestsProviderService.SearchProvidersAsync(filter, patientId);
            return Ok(result);
        }

        private async Task<int> GetPatientIdAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!string.IsNullOrEmpty(userId))
                {
                    var profile = await _patientService.GetProfileByUserID(userId);
                    if (profile != null) return profile.Id;
                }
            }
            return 0;
        }
    }
}