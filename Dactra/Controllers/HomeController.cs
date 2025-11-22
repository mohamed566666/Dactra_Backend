using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly Services.Interfaces.IHomeService _homeService;
        public HomeController(Services.Interfaces.IHomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet("topRatedDoctors/{count}")]
        public async Task<IActionResult> GetTopRatedDoctors(int count = 10)
        {
            var topRatedDoctors = await _homeService.GetTopRatedDoctorsAsync(count);
            return Ok(topRatedDoctors);
        }
    }
}
