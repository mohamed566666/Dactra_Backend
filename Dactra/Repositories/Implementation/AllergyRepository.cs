namespace Dactra.Repositories.Implementation
{
    public class AllergyRepository : GenericRepository<Allergy>, IAllergyRepository
    {
        public AllergyRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Allergy?> GetByNameAsync(string name)
        {
            return await _context.Allergies
                .FirstOrDefaultAsync(a => a.Name == name);
        }
    }
}
