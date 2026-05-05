namespace Dactra.Repositories.Implementation
{
    public class MedicalReportRepository : IMedicalReportRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalReport>> GetByPatientIdAsync(int patientProfileId)
        {
            return await _context.MedicalReports
                .Where(r => r.PatientProfileId == patientProfileId)
                .OrderByDescending(r => r.UploadedAt)
                .ToListAsync();
        }

        public async Task<MedicalReport?> GetByIdAsync(int id)
        {
            return await _context.MedicalReports
                .Include(r => r.PatientProfile)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(MedicalReport report)
        {
            await _context.MedicalReports.AddAsync(report);
        }

        public void Delete(MedicalReport report)
        {
            _context.MedicalReports.Remove(report);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}