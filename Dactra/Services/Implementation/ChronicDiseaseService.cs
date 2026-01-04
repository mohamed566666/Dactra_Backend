namespace Dactra.Services.Implementation
{
    public class ChronicDiseaseService : IChronicDiseaseService
    {
        private readonly IChronicDiseaseRepository _repo;

        public ChronicDiseaseService(IChronicDiseaseRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ChronicDisease>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<ChronicDisease?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task AddAsync(string name)
        {
            if (await _repo.GetByNameAsync(name) != null)
                return;

            await _repo.AddAsync(new ChronicDisease { Name = name });
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, string name)
        {
            var disease = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Disease not found");

            disease.Name = name;
            _repo.Update(disease);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var disease = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Disease not found");

            _repo.Delete(disease);
            await _repo.SaveChangesAsync();
        }
    }
}
