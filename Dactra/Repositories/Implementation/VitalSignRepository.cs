namespace Dactra.Repositories.Implementation
{
    public class VitalSignRepository : GenericRepository<VitalSign>, IVitalSignRepository
    {
        public VitalSignRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<VitalSign>> GetByPatientIdAsync(int patientId)
        {
            return await _context.VitalSigns
                .Where(v => v.PatientId == patientId)
                .Include(v => v.Type)
                .OrderByDescending(v => v.RecordedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<VitalSignType>> GetAllTypesAsync()
        {
            return await _context.VitalSignTypes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<VitalSignType?> GetTypeByIdAsync(int id)
        {
            return await _context.VitalSignTypes
                .Where(t => t.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task AddTypeAsync(VitalSignType type)
        {
            await _context.VitalSignTypes.AddAsync(type);
        }

        public async Task<VitalSignType?> GetTypeByNameAsync(string name)
        {
            var normalizedName = name.ToLower();
            return await _context.VitalSignTypes
                .Where(t => t.Name.ToLower() == normalizedName)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}