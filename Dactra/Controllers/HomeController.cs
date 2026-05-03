using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;
        private readonly ISiteReviewService _siteReviewService;
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;

        public HomeController(IHomeService homeService, ISiteReviewService siteReviewService, ILogger<HomeController> logger, IMemoryCache cache)
        {
            _homeService = homeService;
            _siteReviewService = siteReviewService;
            _logger = logger;
            _cache = cache;
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
                string cacheKey = $"TopRatedDoctors_{count}";

                if (!_cache.TryGetValue(cacheKey, out IEnumerable<TopRatedDoctorDTO> doctors))
                {
                    doctors = await _homeService.GetTopRatedDoctorsAsync(count);

                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                    };

                    _cache.Set(cacheKey, doctors, cacheOptions);
                }

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
            const string cacheKey = "SiteReviewStatistics";

            if (!_cache.TryGetValue(cacheKey, out var stats))
            {
                stats = await _siteReviewService.GetReviewDistributionAsync();

                _cache.Set(cacheKey, stats, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                });
            }

            return Ok(stats);
        }
    }
}