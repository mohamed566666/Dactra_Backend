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
            return await _context.Set<Rating>().Where(r =>
                r.PatientId == patientId &&
                r.ProviderId == providerId)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                        .FirstOrDefaultAsync();
        }

        public async Task<List<Rating>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<Rating>()
                .Where(r => r.PatientId == patientId)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetByProviderIdAsync(int providerId)
        {
            return await _context.Set<Rating>()
                .Where(r => r.ProviderId == providerId)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }
    }
}