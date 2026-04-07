using Dactra.DTOs.CommentDTOs;
using Dactra.DTOs.PostDTOs;
using Dactra.DTOs.PostLikeDTOs;
using Dactra.DTOs.TagDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IPostLikeService _likeService;
        private readonly ISavedPostService _savedPostService;
        private readonly ITagService _tagService;
        private readonly IDoctorService _doctorService;
        private readonly ICommentLikeService _commentLikeService;

        public PostsController(
            IPostService postService,
            ICommentService commentService,
            IPostLikeService likeService,
            ISavedPostService savedPostService,
            ITagService tagService,
            IDoctorService doctorService,
            ICommentLikeService commentLikeService)
        {
            _postService = postService;
            _commentService = commentService;
            _likeService = likeService;
            _savedPostService = savedPostService;
            _tagService = tagService;
            _doctorService = doctorService;
            _commentLikeService = commentLikeService;
        }

        // ── Posts ────────────────────────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _postService.GetAllAsync(page, pageSize, userId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostResponseDto>> GetById(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _postService.GetByIdAsync(id, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("doctor/{doctorId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetByDoctor(
            int doctorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _postService.GetByDoctorIdAsync(doctorId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("my-posts")]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetMyPosts([FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            try
            {
                var doctorId = await GetDoctorId();
                return Ok(await _postService.GetByDoctorIdAsync(doctorId, page, pageSize));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("tag/{tagId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetByTag(
            int tagId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _postService.GetByTagAsync(tagId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PostResponseDto>> Create([FromBody] CreatePostDto dto)
        {
            try
            {
                var doctorId = await GetDoctorId();
                var result = await _postService.CreateAsync(dto, doctorId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PostResponseDto>> Update(int id, [FromBody] UpdatePostDto dto)
        {
            try
            {
                var doctorId = await GetDoctorId();
                return Ok(await _postService.UpdateAsync(id, dto, doctorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var doctorId = await GetDoctorId();
                await _postService.DeleteAsync(id, doctorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Comments ─────────────────────────────────────────────────────────────

        [HttpGet("{postId:int}/comments")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CommentResponseDto>>> GetComments(int postId)
        {
            var userId = GetUserId();
            try
            {
                return Ok(await _commentService.GetByPostIdAsync(postId, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{postId:int}/comments")]
        public async Task<ActionResult<CommentResponseDto>> AddComment(int postId, [FromBody] CreateCommentDto dto)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _commentService.CreateAsync(postId, dto, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("comments/{commentId:int}")]
        public async Task<ActionResult<CommentResponseDto>> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _commentService.UpdateAsync(commentId, dto, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userId = GetUserId();
                await _commentService.DeleteAsync(commentId, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Likes ─────────────────────────────────────────────────────────────────

        [HttpPost("{postId:int}/like")]
        [AllowAnonymous]
        public async Task<ActionResult<PostLikeResponseDto>> ToggleLike(int postId)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _likeService.ToggleLikeAsync(postId, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Saved Posts ───────────────────────────────────────────────────────────

        [HttpPost("{postId:int}/save")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> ToggleSave(int postId)
        {
            try
            {
                var userId = GetUserId();
                var isSaved = await _savedPostService.ToggleSaveAsync(postId, userId);
                return Ok(new { postId, isSaved });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("saved")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<SavedPostResponseDto>>> GetSaved(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _savedPostService.GetSavedPostsAsync(userId, page, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ── Tags ──────────────────────────────────────────────────────────────────

        [HttpGet("tags")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> GetAllTags()
        {
            try
            {
                return Ok(await _tagService.GetAllTagsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{postId:int}/tags")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> GetPostTags(int postId)
        {
            try
            {
                return Ok(await _tagService.GetTagsByPostIdAsync(postId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filterOn")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<PostResponseDto>>> GetMyPosts([FromQuery] PostFilterDto filter,[FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _postService.GetMyFilteredPostsAsync(filter, userId, page, pageSize));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("comments/{commentId:int}/like")]
        [AllowAnonymous]
        public async Task<ActionResult<CommentLikeResponseDto>> ToggleCommentLike(int commentId)
        {
            try
            {
                var userId = GetUserId();
                return Ok(await _commentLikeService.ToggleLikeAsync(commentId, userId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private string? GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userId))
            //    throw new UnauthorizedAccessException("User not authenticated.");
            return userId;
        }

        private async Task<int> GetDoctorId()
        {
            var userId = GetUserId();
            var profile = await _doctorService.GetProfileByUserIdAsync(userId);
            if (profile == null)
                throw new UnauthorizedAccessException("Doctor profile not found.");
            return profile.Id;
        }
    }
}
