namespace Dactra.Repositories.Implementation
{
    public class PatientProfileRepository : GenericRepository<PatientProfile> , IPatientProfileRepository
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
                .ToListAsync();
        }
        public override async Task<PatientProfile?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include (p => p.User)
                .Include(p => p.Address)
                .Include(p => p.Allergies)
                .Include(p => p.ChronicDiseases)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PatientProfile?> GetByUserEmail(string email)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Address)
                .Include(p => p.Allergies)
                .Include(p => p.ChronicDiseases)
                .FirstOrDefaultAsync(p => p.User.Email == email);
        }

        public async Task<PatientProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Address)
                .Include(p => p.Allergies)
                .Include(p => p.ChronicDiseases)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
