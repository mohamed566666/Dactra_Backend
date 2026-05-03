namespace Dactra.Repositories.Implementation
{
    public class PatientProfileRepository : GenericRepository<PatientProfile>, IPatientProfileRepository
    {
        public PatientProfileRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<PatientProfile>> GetAllAsync()
        {
            return await _context.Patients
             .Include(p => p.User)
             .Include(p => p.Address)
             .Include(p => p.Allergies)
             .Include(p => p.ChronicDiseases)
             .Include(p => p.MedicalReports)
             .AsSplitQuery()
             .AsNoTracking()
             .ToListAsync();
        }
        public override async Task<PatientProfile?> GetByIdAsync(int id)
        {
            return await _context.Patients
             .Where(p => p.Id == id)
             .Include(p => p.User)
             .Include(p => p.Address)
             .Include(p => p.Allergies)
             .Include(p => p.ChronicDiseases)
             .Include(p => p.MedicalReports)
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync();
        }

        public async Task<PatientProfile?> GetByUserEmail(string email)
        {
            return await _context.Patients
                 .Where(p => p.User.Email == email)
                 .Include(p => p.User)
                 .Include(p => p.Address)
                 .Include(p => p.Allergies)
                 .Include(p => p.ChronicDiseases)
                 .Include(p => p.MedicalReports)
                 .AsSplitQuery()
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
        }

        public async Task<PatientProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Patients
                 .Where(p => p.UserId == userId)
                 .Include(p => p.User)
                 .Include(p => p.Address)
                 .Include(p => p.Allergies)
                 .Include(p => p.ChronicDiseases)
                 .Include(p => p.MedicalReports)
                 .AsSplitQuery()
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
        }
    }
}