namespace Dactra.Repositories.Implementation
{
    public class VitalSignRepository : GenericRepository<VitalSign>, IVitalSignRepository
    {
        public VitalSignRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<VitalSign>> GetByPatientIdAsync(int patientId)
        {
            return await _context.VitalSigns
            .Include(v => v.Type)
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.RecordedAt)
            .ToListAsync();
        }

        public async Task<List<VitalSignType>> GetAllTypesAsync()
        {
            return await _context.VitalSignTypes.ToListAsync();
        }

        public async Task<VitalSignType?> GetTypeByIdAsync(int id)
        {
            return await _context.VitalSignTypes.FirstOrDefaultAsync(t => t.Id == id);
        }
        
        public async Task AddTypeAsync(VitalSignType type)
        {
            await _context.VitalSignTypes.AddAsync(type);
        }

        public async Task<VitalSignType?> GetTypeByNameAsync(string name)
        {
            return await _context.VitalSignTypes
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }
    }
}
