namespace Dactra.Repositories.Implementation
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceProviderProfile?> GetByIdAsync(int id)
        {
            return await _context.Providers
                .AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<ServiceProviderProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Providers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}