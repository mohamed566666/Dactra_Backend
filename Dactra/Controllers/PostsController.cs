using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            var createdPost = await _postService.CreateAsync(post);
            return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, createdPost);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto)
        {
            try
            {
                int doctorId = User.GetDoctorId(); 

                var updated = await _postService.UpdateAsync(id, doctorId, dto);
                if (!updated) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int doctorId = User.GetDoctorId(); 

                var deleted = await _postService.DeleteAsync(id, doctorId);
                if (!deleted) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); 
            }
        }
    }
}
