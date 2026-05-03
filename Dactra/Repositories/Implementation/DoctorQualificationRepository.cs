namespace Dactra.Repositories.Implementation
{
    public class DoctorQualificationRepository : GenericRepository<DoctorQualification>, IDoctorQualificationRepository
    {
        public DoctorQualificationRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<DoctorQualification>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorQualifications
                .Where(dq => dq.DoctorProfileId == doctorId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Where(d => d.UserId == userId && !d.User.isDeleted)
                .Include(d => d.Qualifications)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}