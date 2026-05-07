using Microsoft.Extensions.Caching.Memory;

namespace Dactra.Services.Implementation
{
    public class ChronicDiseaseService : IChronicDiseaseService
    {
        private readonly IChronicDiseaseRepository _repo;
        private readonly IMemoryCache _cache;
        private const string ALL_DISEASES_CACHE_KEY = "AllChronicDiseases";

        public ChronicDiseaseService(IChronicDiseaseRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<IEnumerable<ChronicDisease>> GetAllAsync()
        {
            return await _cache.GetOrCreateAsync(ALL_DISEASES_CACHE_KEY, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _repo.GetAllAsync();
            });
        }

        public async Task<ChronicDisease?> GetByIdAsync(int id)
        {
            string cacheKey = $"ChronicDisease_{id}";

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

            await _repo.AddAsync(new ChronicDisease { Name = name });
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_DISEASES_CACHE_KEY);
        }

        public async Task UpdateAsync(int id, string name)
        {
            var disease = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Disease not found");

            disease.Name = name;
            _repo.Update(disease);
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_DISEASES_CACHE_KEY);
            _cache.Remove($"ChronicDisease_{id}");
        }

        public async Task DeleteAsync(int id)
        {
            var disease = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Disease not found");

            _repo.Delete(disease);
            await _repo.SaveChangesAsync();

            _cache.Remove(ALL_DISEASES_CACHE_KEY);
            _cache.Remove($"ChronicDisease_{id}");
        }
    }
}