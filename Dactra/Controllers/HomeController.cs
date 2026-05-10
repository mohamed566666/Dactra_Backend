using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;
        private readonly ISiteReviewService _siteReviewService;
        private readonly IPatientService _patientService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHomeService homeService, ISiteReviewService siteReviewService, IPatientService patientService, ILogger<HomeController> logger)
        {
            _homeService = homeService;
            _siteReviewService = siteReviewService;
            _patientService = patientService;
            _logger = logger;
        }

        [HttpGet("top-rated-doctors")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<TopRatedDoctorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TopRatedDoctorDTO>>> GetTopRatedDoctors([FromQuery] int count = 10)
        {
            try
            {
                int patientId = 0;

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var profile = await _patientService.GetProfileByUserID(userId);
                        if (profile != null)
                        {
                            patientId = profile.Id;
                        }
                    }
                }

                var doctors = await _homeService.GetTopRatedDoctorsAsync(count, patientId);
                return Ok(doctors);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid parameter: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching top rated doctors");
                return StatusCode(500, new { message = "an Error occurred while fetching the Data." });
            }
        }

        [HttpGet("review-Statistics")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewDistribution()
        {
            var dto = await _siteReviewService.GetReviewDistributionAsync();
            return Ok(dto);
        }
    }
}