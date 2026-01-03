namespace Dactra.Repositories.Implementation
{
    public class ChronicDiseaseRepository : GenericRepository<ChronicDisease>, IChronicDiseaseRepository
    {
        public ChronicDiseaseRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<ChronicDisease?> GetByNameAsync(string name)
        {
            return await _context.ChronicDiseases
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
