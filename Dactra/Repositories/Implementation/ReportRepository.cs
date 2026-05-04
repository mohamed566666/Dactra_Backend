namespace Dactra.Repositories.Implementation
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteAsync(Report report)
        {
            _context.Reports.Remove(report);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}