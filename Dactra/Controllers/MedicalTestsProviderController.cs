using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;
using Dactra.Enums;
using Dactra.Services.Implementation;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalTestsProviderController : ControllerBase
    {
        private readonly IMedicalTestsProviderService _medicalTestsProviderService;
        public MedicalTestsProviderController(IMedicalTestsProviderService medicalTestsProviderService)
        {
            _medicalTestsProviderService = medicalTestsProviderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetAllProfilesAsync();
            return Ok(MedicalTestProviderProfiles);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            try
            {
                var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByIdAsync(Id);
                return Ok(MedicalTestProviderProfile);
            }
            catch (Exception ex) { 
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(MedicalTestsProviderUpdateDTO medicalTestProviderDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            try
            {
                await _medicalTestsProviderService.UpdateProfileAsync(userId, medicalTestProviderDTO);
                return Ok("Profile Updated Succesfully");
            }
            catch (Exception ex) {
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
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpGet ("GetByUserId/{UserId}")]
        public async Task<IActionResult> GetProfileByUserId(string UserId)
        {
            try
            {
                var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByUserIdAsync(UserId);
                return Ok(MedicalTestProviderProfile);
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetApproved")]
        public async Task<IActionResult> GetApproved()
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetApprovedProfilesAsync();
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpGet("GetByType{type}")]
        public async Task<IActionResult> GetByType(MedicalTestProviderType type)
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetProfilesByTypeAsync(type);
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
        [HttpPatch("Approve/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeProfileStatus(int Id)
        {
            try
            {
                await _medicalTestsProviderService.ApproveProfileAsync(Id);
                return Ok("Profile Approved Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("Reject/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProfile(int Id)
        {
            try
            {
                await _medicalTestsProviderService.RejectProfileAsync(Id);
                return Ok("Profile Rejected Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteProfile(int Id)
        {
            try
            {
                await _medicalTestsProviderService.DeleteMedicalTestProviderProfileAsync(Id);
                return Ok("Profile Deleted Succesfully");
            }
            catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
    }
}
