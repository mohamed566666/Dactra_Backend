using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyService _service;
        private readonly IMemoryCache _cache;

        private const string AllergiesCacheKey = "AllergiesList";

        public AllergyController(IAllergyService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!_cache.TryGetValue(AllergiesCacheKey, out var cachedAllergies))
            {
                cachedAllergies = await _service.GetAllAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };

                _cache.Set(AllergiesCacheKey, cachedAllergies, cacheOptions);
            }

            return Ok(cachedAllergies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            await _service.AddAsync(name);

            _cache.Remove(AllergiesCacheKey);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, string name)
        {
            await _service.UpdateAsync(id, name);

            _cache.Remove(AllergiesCacheKey);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            _cache.Remove(AllergiesCacheKey);

            return Ok();
        }
    }
}