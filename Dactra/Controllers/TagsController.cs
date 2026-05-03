using Dactra.DTOs.TagDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMemoryCache _cache;

        private const string AllTagsCacheKey = "AllTags";

        public TagsController(ITagService tagService, IMemoryCache cache)
        {
            _tagService = tagService;
            _cache = cache;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<TagDto>>> GetAll()
        {
            try
            {
                if (!_cache.TryGetValue(AllTagsCacheKey, out List<TagDto> tags))
                {
                    tags = await _tagService.GetAllTagsAsync();
                    _cache.Set(AllTagsCacheKey, tags, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                }
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagDto dto)
        {
            try
            {
                var result = await _tagService.CreateTagAsync(dto);
                _cache.Remove(AllTagsCacheKey);
                return CreatedAtAction(nameof(GetAll), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);
                _cache.Remove(AllTagsCacheKey);
                return NoContent();
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
    }
}