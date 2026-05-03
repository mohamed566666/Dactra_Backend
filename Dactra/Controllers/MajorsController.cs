using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly IMajorsService _majorsService;
        private readonly IMemoryCache _cache;

        private const string MajorsListCacheKey = "MajorsList";

        public MajorsController(IMajorsService majorsService, IMemoryCache cache)
        {
            _majorsService = majorsService;
            _cache = cache;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] MajorBasicsDTO major)
        {
            await _majorsService.CreateMajorAsync(major);
            _cache.Remove(MajorsListCacheKey);
            return Ok("Major created successfully");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(MajorsListCacheKey, out var majors))
            {
                majors = await _majorsService.GetAllMajorsAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
                    Priority = CacheItemPriority.NeverRemove
                };

                _cache.Set(MajorsListCacheKey, majors, cacheOptions);
            }

            return Ok(majors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            string cacheKey = $"Major_{id}";

            if (!_cache.TryGetValue(cacheKey, out var major))
            {
                major = await _majorsService.GetMajorByIdAsync(id);

                if (major == null)
                {
                    return NotFound("Major Not Found");
                }

                _cache.Set(cacheKey, major, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
                    Priority = CacheItemPriority.NeverRemove
                });
            }

            return Ok(major);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] MajorBasicsDTO major)
        {
            await _majorsService.UpdateMajorAsync(id, major);
            _cache.Remove(MajorsListCacheKey);
            _cache.Remove($"Major_{id}");
            return Ok("Major Updated successfully");
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateIcon(int id, string iconUrl)
        {
            await _majorsService.UpdateMajorIconAsync(id, iconUrl);
            _cache.Remove(MajorsListCacheKey);
            _cache.Remove($"Major_{id}");
            return Ok("Major icon updated successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _majorsService.DeleteMajorByIdAsync(id);
            _cache.Remove(MajorsListCacheKey);
            _cache.Remove($"Major_{id}");
            return Ok("Major deleted successfully");
        }
    }
}