using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChronicDiseaseController : ControllerBase
    {
        private readonly IChronicDiseaseService _service;
        private readonly IMemoryCache _cache;

        private const string ChronicDiseasesCacheKey = "ChronicDiseasesList";

        public ChronicDiseaseController(IChronicDiseaseService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(ChronicDiseasesCacheKey, out var cachedDiseases))
            {
                cachedDiseases = await _service.GetAllAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                    Priority = CacheItemPriority.NeverRemove
                };

                _cache.Set(ChronicDiseasesCacheKey, cachedDiseases, cacheOptions);
            }

            return Ok(cachedDiseases);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            await _service.AddAsync(name);
            _cache.Remove(ChronicDiseasesCacheKey);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, string name)
        {
            await _service.UpdateAsync(id, name);
            _cache.Remove(ChronicDiseasesCacheKey);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            _cache.Remove(ChronicDiseasesCacheKey);
            return Ok();
        }
    }
}