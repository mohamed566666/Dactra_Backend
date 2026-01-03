namespace Dactra.Services.Implementation
{
    public class AllergyService : IAllergyService
    {
        private readonly IAllergyRepository _repo;

        public AllergyService(IAllergyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Allergy>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Allergy?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task AddAsync(string name)
        {
            if (await _repo.GetByNameAsync(name) != null)
                return;
            await _repo.AddAsync(new Allergy { Name = name });
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, string name)
        {
            var allergy = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Allergy not found");
            allergy.Name = name;
            _repo.Update(allergy);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var allergy = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Allergy not found");
            _repo.Delete(allergy);
            await _repo.SaveChangesAsync();
        }
    }
}
