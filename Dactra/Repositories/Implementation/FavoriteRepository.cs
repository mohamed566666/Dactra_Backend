namespace Dactra.Repositories.Implementation
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ToggleFavoriteAsync(int patientId, int serviceProviderId)
        {
            var favorite = await _context.Set<PatientFavoriteServiceProvider>().FindAsync(patientId, serviceProviderId);

            if (favorite != null)
            {
                _context.Set<PatientFavoriteServiceProvider>().Remove(favorite);
            }
            else
            {
                var exists = await _context.Set<ServiceProviderProfile>().AnyAsync(p => p.Id == serviceProviderId);
                if (!exists) throw new KeyNotFoundException("Service provider not found");

                await _context.Set<PatientFavoriteServiceProvider>().AddAsync(new PatientFavoriteServiceProvider
                {
                    PatientId = patientId,
                    ServiceProviderId = serviceProviderId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(int TotalCount, List<DoctorProfile> Items)> GetFavoriteDoctorsPagedAsync(int patientId, int skip, int take)
        {
            var providerIds = await _context.Set<PatientFavoriteServiceProvider>()
                .Where(f => f.PatientId == patientId)
                .Select(f => f.ServiceProviderId)
                .ToListAsync();

            var query = _context.Set<DoctorProfile>().Where(d => providerIds.Contains(d.Id));
            var totalCount = await query.CountAsync();

            var items = await query
                .Include(d => d.User)
                .Include(d => d.specialization)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<MedicalTestProviderProfile> Items)> GetFavoriteProvidersPagedAsync(int patientId, int skip, int take)
        {
            var providerIds = await _context.Set<PatientFavoriteServiceProvider>()
                .Where(f => f.PatientId == patientId)
                .Select(f => f.ServiceProviderId)
                .ToListAsync();

            var query = _context.Set<MedicalTestProviderProfile>().Where(p => providerIds.Contains(p.Id));
            var totalCount = await query.CountAsync();

            var items = await query
                .Include(p => p.User)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (totalCount, items);
        }
    }
}
