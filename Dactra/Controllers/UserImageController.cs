using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserImageController : ControllerBase
    {
        private readonly IUserImageService _userImageService;

        public UserImageController(IUserImageService userImageService)
        {
            _userImageService = userImageService;
        }

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _userImageService.GetAsync(CurrentUserId);
            if (result == null)
                return NotFound(new { message = "User not found" });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            var (success, url, error) = await _userImageService.UploadOrReplaceAsync(CurrentUserId, file);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Image uploaded successfully", imageUrl = url });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var (success, error) = await _userImageService.DeleteAsync(CurrentUserId);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Image deleted successfully" });
        }

        [HttpPut]
        public async Task<IActionResult> Replace(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });
            var (success, url, error) = await _userImageService.UploadOrReplaceAsync(CurrentUserId, file);
            if (!success)
                return BadRequest(new { message = error });
            return Ok(new { message = "Image replaced successfully", imageUrl = url });
        }
    }
}
