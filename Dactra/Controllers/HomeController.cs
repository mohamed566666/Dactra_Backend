using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;
        private readonly ISiteReviewService _siteReviewService;
        public HomeController(IHomeService homeService , ISiteReviewService siteReviewService)
        {
            _homeService = homeService;
            _siteReviewService = siteReviewService;
        }
        [HttpGet("topRatedDoctors/{count}")]
        public async Task<IActionResult> GetTopRatedDoctors(int count = 10)
        {
            var topRatedDoctors = await _homeService.GetTopRatedDoctorsAsync(count);
            return Ok(topRatedDoctors);
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
