namespace Dactra.Repositories.Implementation
{
    public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Complaint>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Complaint>> GetAllWithAttachmentsAsync()
        {
            return await _dbSet
                .Include(c => c.Attachments)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Complaint?> GetDetailsAsync(int id)
        {
            return await _dbSet
                .Where(c => c.Id == id)
                .Include(c => c.Attachments)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}