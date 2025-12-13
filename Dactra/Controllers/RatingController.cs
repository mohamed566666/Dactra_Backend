using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IPatientService _patientService;
        private readonly IServiceProviderService _ServiceProviderService;
        public RatingController(IRatingService ratingService, IPatientService patientService , IServiceProviderService serviceProviderService)
        {
            _ratingService = ratingService;
            _patientService = patientService;
            _ServiceProviderService = serviceProviderService;
        }
        [HttpPost("patient/rate-provider/{providerId}")]
        [Authorize]
        public async Task<IActionResult> RateProvider(int providerId, [FromBody] CreateRatingDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _patientService.GetProfileByUserID(userId);
            var result = await _ratingService.RateProviderAsync(profile.Id, providerId, dto.Score, dto.Comment);
            if (!result)
                return BadRequest("You already rated this provider");
            return Ok("Rate done successfully");
        }

        [HttpPut("patient/rate-provider/{providerId}")]
        [Authorize]
        public async Task<IActionResult> UpdateRating(int providerId, [FromBody] UpdateRatingDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _patientService.GetProfileByUserID(userId);
            var result = await _ratingService.UpdateRatingAsync(profile.Id, providerId, dto.Score, dto.Comment);
            if (!result)
                return NotFound("Rating not found");
            return Ok("Rating updated successfully");
        }

        [HttpDelete("patient/rate-provider/{providerId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRating(int providerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _patientService.GetProfileByUserID(userId);
            var result = await _ratingService.DeleteRatingAsync(profile.Id, providerId);
            if (!result)
                return NotFound("Rating not found");
            return Ok("Rating deleted successfully");
        }

        [HttpGet("patient/my-ratings")]
        [Authorize]
        public async Task<IActionResult> GetMyRatings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _patientService.GetProfileByUserID(userId);
            var ratings = await _ratingService.GetRatingsByPatientAsync(profile.Id);
            return Ok(ratings);
        }

        [HttpGet("patient/my-rating/{providerId}")]
        [Authorize]
        public async Task<IActionResult> GetMyRatingForProvider(int providerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _patientService.GetProfileByUserID(userId);
            var rating = await _ratingService.GetRatingByPatientAndProviderAsync(profile.Id, providerId);
            return rating == null ? NotFound("Rating not found") : Ok(rating);
        }
        [HttpGet("provider/my-ratings")]
        [Authorize]
        public async Task<IActionResult> GetMyRatingsForProvider()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var provider = await _ServiceProviderService.GetByUserIdAsync(userId);
            if (provider == null)
                return NotFound("Provider profile not found");
            var ratings = await _ratingService.GetRatingsforProviderAsync(provider.Id);
            return Ok(ratings);
        }
    }
}
