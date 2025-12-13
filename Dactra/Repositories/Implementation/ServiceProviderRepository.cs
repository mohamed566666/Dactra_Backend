namespace Dactra.Repositories.Implementation
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceProviderProfile?> GetByIdAsync(ServiceProviderType type, int id)
        {
            return type switch
            {
                ServiceProviderType.Doctor =>
                    await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id && !d.User.isDeleted),
                ServiceProviderType.MedicalTestProvider =>
                    await _context.Doctors.FirstOrDefaultAsync(m => m.Id == id && !m.User.isDeleted),
                _ => null
            };
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
