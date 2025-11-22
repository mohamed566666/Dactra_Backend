using Dactra.DTOs.ProfilesDTO;
using Dactra.DTOs.ProfilesDTOs;
using Dactra.Enums;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister(DTOs.ProfilesDTO.MedicalTestProviderDTO medicalTestProviderDTO)
        {
            await _medicalTestsProviderService.CompleteRegistrationAsync(medicalTestProviderDTO);
            return Ok();
        }

        [HttpGet("GetAllProfiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetAllProfilesAsync();
            return Ok(MedicalTestProviderProfiles);
        }
        [HttpGet("GetProfileById/{Id}")]
        public async Task<IActionResult> GetProfileById(int Id)
        {
            var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByIdAsync(Id);
            return Ok(MedicalTestProviderProfile);
        }
        [HttpGet ("GetProfileByUserId/{UserId}")]
        public async Task<IActionResult> GetProfileByUserId(string UserId)
        {
            var MedicalTestProviderProfile = await _medicalTestsProviderService.GetProfileByUserIdAsync(UserId);
            return Ok(MedicalTestProviderProfile);
        }

        [HttpGet("GetApprovedProfiles")]
        public async Task<IActionResult> GetApprovedProfiles()
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetApprovedProfilesAsync();
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpGet("GetProfilesByType")]
        public async Task<IActionResult> GetProfilesByType(MedicalTestProviderType type)
        {
            var MedicalTestProviderProfiles = await _medicalTestsProviderService.GetProfilesByTypeAsync(type);
            return Ok(MedicalTestProviderProfiles);
        }

        [HttpPut("UpdateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id,MedicalTestsProviderUpdateDTO medicalTestProviderDTO)
        {
            await _medicalTestsProviderService.UpdateProfileAsync(id, medicalTestProviderDTO);
            return Ok("Profile Updated Succesfully");
        }
        [HttpPatch("ApproveProfile/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeProfileStatus(int Id)
        {
            await _medicalTestsProviderService.ApproveProfileAsync(Id);
            return Ok("Profile Approved Succesfully");
        }

        [HttpPatch("RejectProfile/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProfile(int Id)
        {
            await _medicalTestsProviderService.RejectProfileAsync(Id);
            return Ok("Profile Rejected Succesfully");
        }

        [HttpDelete("DeleteProfile/{Id}")]
        public async Task<IActionResult> DeleteProfile(int Id)
        {
            await _medicalTestsProviderService.DeleteMedicalTestProviderProfileAsync(Id);
            return Ok("Profile Deleted Succesfully");
        }
    }
}
