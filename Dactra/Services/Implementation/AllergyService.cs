using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Services.Implementation
{
    public class AllergyService : IAllergyService
    {
        private readonly IAllergyRepository _repo;
        private readonly IMemoryCache _cache;
        private const string ALL_ALLERGIES_CACHE_KEY = "AllAllergies";

        public AllergyService(IAllergyRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<IEnumerable<Allergy>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(ALL_ALLERGIES_CACHE_KEY, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _repo.GetAllAsync();
            });
        }

        public async Task<Allergy?> GetByIdAsync(int id)
        {
            string cacheKey = $"Allergy_{id}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _repo.GetByIdAsync(id);
            });
        }

        public async Task AddAsync(string name)
        {
            if (await _repo.GetByNameAsync(name) != null)
                return;

            await _repo.AddAsync(new Allergy { Name = name });
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_ALLERGIES_CACHE_KEY);
        }

        public async Task UpdateAsync(int id, string name)
        {
            var allergy = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Allergy not found");

            allergy.Name = name;
            _repo.Update(allergy);
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_ALLERGIES_CACHE_KEY);
            _cache.Remove($"Allergy_{id}");
        }

        public async Task DeleteAsync(int id)
        {
            var allergy = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Allergy not found");

            _repo.Delete(allergy);
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_ALLERGIES_CACHE_KEY);
            _cache.Remove($"Allergy_{id}");
        }
    }
}