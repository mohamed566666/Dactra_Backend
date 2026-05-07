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
                var reviews = await _service.GetAllReviewsAsync();
                var existing = reviews.FirstOrDefault(r => r.ReviewerName == User.Identity.Name);
                if (existing != null)
                {
                    existing.Title = dto.Title;
                    existing.Score = dto.Score;
                    existing.Comment = dto.Comment;
                    await _service.UpdateReviewAsync(userId!, existing.Id, dto);
                    return Ok(new { success = true, message = "Review Updated Successfully",
                        review = new
                        {
                            existing.Id,
                            existing.Title,
                            existing.Score,
                            existing.Comment,
                            existing.CreatedAt,
                            existing.ReviewerName,
                            existing.ReviewerImageUrl
                        }
                    });
                }
                var created = await _service.CreateReviewAsync(userId!, dto);
                return Ok(new { success = true, message = "Review Added Successfully",
                    review = new
                    {
                        created.Id,
                        created.Title,
                        created.Score,
                        created.Comment,
                        created.CreatedAt,
                        created.ReviewerName,
                        created.ReviewerImageUrl
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var reviews = await _service.GetAllReviewsAsync();
            var single = reviews.FirstOrDefault(r => r.Id == id);
            if (single == null)
                return NotFound(new { success = false, message = "Review not found" });
            return Ok(new { success = true, data = single });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] SiteReviewRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _service.UpdateReviewAsync(userId!, id, dto);
                return Ok(new { success = true, message = "Review Updated Successfully"});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { success = false, message = "You are not authorized to update this review" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var items = await _service.GetAllReviewsAsync();
            return Ok(new { success = true, count = items.Count(), data = items });
        }

        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<IActionResult> Stats()
        {
            var (count, avg) = await _service.GetStatsAsync();
            return Ok(new { success = true, data = new { count, avg } });
        }

        [HttpGet("distribution")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistribution()
        {
            var distribution = await _service.GetReviewDistributionAsync();
            return Ok(new { success = true, data = distribution });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _service.DeleteReviewAsync(userId!, id);
                return Ok(new { success = true, message = "Review deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { success = false, message = "You are not authorized to delete this review" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
