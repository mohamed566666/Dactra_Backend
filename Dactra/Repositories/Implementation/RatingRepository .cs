namespace Dactra.Repositories.Implementation
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Rating?> GetByPatientAndProviderAsync(int patientId, int providerId)
        {
            return await _context.Set<Rating>().FirstOrDefaultAsync(r =>
                r.PatientId == patientId &&
                r.ProviderId == providerId);
        }

        public async Task<List<Rating>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<Rating>()
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetByProviderIdAsync(int providerId)
        {
            return await _context.Set<Rating>()
                .Where(r => r.ProviderId == providerId)
                .ToListAsync();
        }
    }
}
