using Dactra.DTOs.SiteReviewDTOs;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteReviewsController : ControllerBase
    {
        private readonly ISiteReviewService _service;

        public SiteReviewsController(ISiteReviewService service)
        {
            _service = service;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] SiteReviewRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var created = await _service.CreateReviewAsync(userId!, dto);
                return Ok("Reviw Added Successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _service.GetAllReviewsAsync();
            var single = review.FirstOrDefault(r => r.Id == id);
            if (single == null) return NotFound(new { success = false, message = "Review not found" });
            return Ok(single);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] SiteReviewRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _service.UpdateReviewAsync(userId!, id, dto);
                return Ok("Reviw Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var items = await _service.GetAllReviewsAsync();
            return Ok(items);
        }

        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<IActionResult> Stats()
        {
            var (count, avg) = await _service.GetStatsAsync();
            return Ok(new { count, avg });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _service.DeleteReviewAsync(userId!, id);
                return Ok(new { success = true, message = "Review deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
