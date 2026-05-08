using Dactra.DTOs.Sponsorship;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientReferralController : ControllerBase
    {
        private readonly IPatientReferralService _service;
        private readonly IDoctorProfileRepository _doctorRepo;
        private readonly IMedicalTestProviderProfileRepository _providerRepo;

        public PatientReferralController(
            IPatientReferralService service,
            IDoctorProfileRepository doctorRepo,
            IMedicalTestProviderProfileRepository providerRepo)
        {
            _service = service;
            _doctorRepo = doctorRepo;
            _providerRepo = providerRepo;
        }

        // ─── Doctor Endpoints ──────────────────────────────────────

        [HttpGet("doctor/care-patients")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetCarePatients([FromQuery] PaginationDto pagination,[FromQuery] string? searchTerm = null)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.GetDoctorCarePatientsAsync(doctor.Id, pagination, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpPost("doctor/refer")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> ReferPatient([FromBody] ReferPatientDTO dto)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.ReferPatientAsync(doctor.Id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Error(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpGet("doctor/referrals")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorReferrals()
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.GetReferralsByDoctorAsync(doctor.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpPut("provider/receive/{referralId}")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> MarkAsReceived(int referralId)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.MarkAsReceivedAsync(referralId, provider.Id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Error(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        // =============== NEW PAGINATED PROVIDER ENDPOINT ===============

        [HttpGet("provider/referrals")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> GetProviderReferralsPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ReferralStatus? status = null)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.GetProviderReferralsPagedAsync(provider.Id, page, pageSize, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        // ─── Helpers ───────────────────────────────────────────────

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private async Task<DoctorProfile?> GetDoctorAsync()
            => await _doctorRepo.GetByUserIdAsync(UserId);

        private async Task<MedicalTestProviderProfile?> GetProviderAsync()
            => await _providerRepo.GetByUserIdAsync(UserId);

        private static object Error(string message, string? details = null) => new
        {
            success = false,
            message,
            details
        };
    }
}