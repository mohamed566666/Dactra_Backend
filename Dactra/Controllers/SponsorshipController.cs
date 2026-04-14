using Dactra.DTOs.Sponsorship;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SponsorshipController : ControllerBase
    {
        private readonly ISponsorshipService _service;
        private readonly IMedicalTestProviderProfileRepository _providerRepo;
        private readonly IDoctorProfileRepository _doctorRepo;

        public SponsorshipController(
            ISponsorshipService service,
            IMedicalTestProviderProfileRepository providerRepo,
            IDoctorProfileRepository doctorRepo)
        {
            _service = service;
            _providerRepo = providerRepo;
            _doctorRepo = doctorRepo;
        }

        // ─── Medical Test Provider Endpoints ──────────────────────

        [HttpPost("provider/offer")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> SendOffer([FromBody] SendOfferDTO dto)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.SendOfferAsync(provider.Id, provider.Type, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(Error(ex.Message));
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

        [HttpGet("provider/offers/summary")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> GetOffersSummary()
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.GetProviderOffersSummaryAsync(provider.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpGet("provider/offers/by-status/{status}")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> GetOffersByStatus(SponsorshipStatus status)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.GetProviderOffersByStatusAsync(provider.Id, status);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpGet("provider/active-sponsors/overview")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> GetActiveSponsorsOverview()
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.GetActiveSponsorsOverviewAsync(provider.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpPut("provider/counter/{sponsorshipId}/accept")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> AcceptCounter(int sponsorshipId)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.AcceptCounterAsync(sponsorshipId, provider.Id);
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

        [HttpPut("provider/counter/{sponsorshipId}/reject")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> RejectCounter(int sponsorshipId)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.RejectCounterAsync(sponsorshipId, provider.Id);
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

        [HttpPut("provider/cancel/{sponsorshipId}")]
        [Authorize(Roles = "MedicalTestProvider")]
        public async Task<IActionResult> ProviderCancelSponsorship(int sponsorshipId)
        {
            try
            {
                var provider = await GetProviderAsync();
                if (provider is null) return Unauthorized(Error("Provider account not found"));

                var result = await _service.CancelActiveSponsorshipAsync(
                    sponsorshipId, provider.Id, "MedicalTestProvider");
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

        // ─── Doctor Endpoints ──────────────────────────────────────

        [HttpGet("doctor/offers")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorOffers()
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.GetDoctorOffersAsync(doctor.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpGet("doctor/active/{type}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetActiveSponsorship(MedicalTestProviderType type)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.GetActiveSponsorshipAsync(doctor.Id, type);
                if (result is null)
                    return NotFound(Error($"No active {type} sponsorship found"));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Error("An unexpected error occurred", ex.Message));
            }
        }

        [HttpPut("doctor/accept/{sponsorshipId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AcceptOffer(int sponsorshipId)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.AcceptOfferAsync(sponsorshipId, doctor.Id);
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

        [HttpPut("doctor/reject/{sponsorshipId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> RejectOffer(int sponsorshipId)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.RejectOfferAsync(sponsorshipId, doctor.Id);
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

        [HttpPut("doctor/counter/{sponsorshipId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CounterOffer(int sponsorshipId, [FromBody] CounterOfferDTO dto)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.CounterOfferAsync(sponsorshipId, doctor.Id, dto);
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

        [HttpPut("doctor/cancel/{sponsorshipId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorCancelSponsorship(int sponsorshipId)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null) return Unauthorized(Error("Doctor account not found"));

                var result = await _service.CancelActiveSponsorshipAsync(
                    sponsorshipId, doctor.Id, "Doctor");
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

        // ─── Helpers ───────────────────────────────────────────────

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private async Task<MedicalTestProviderProfile?> GetProviderAsync()
            => await _providerRepo.GetByUserIdAsync(UserId);

        private async Task<DoctorProfile?> GetDoctorAsync()
            => await _doctorRepo.GetByUserIdAsync(UserId);

        private static object Error(string message, string? details = null) => new
        {
            success = false,
            message,
            details
        };
    }
}