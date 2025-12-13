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
                .ToListAsync();
        }
        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.Qualifications)
                .FirstOrDefaultAsync(d => d.UserId == userId && !d.User.isDeleted);
        }
    }
}
