namespace Dactra.Repositories.Implementation
{
    public class MedicalTestProviderProfileRepository : GenericRepository<MedicalTestProviderProfile>, IMedicalTestProviderProfileRepository
    {
        public MedicalTestProviderProfileRepository(ApplicationDbContext context) : base(context)
        {

        }

        public override async Task<IEnumerable<MedicalTestProviderProfile>> GetAllAsync()
        {
            return await _context.MedicalTestProviders
                .Where(m => m.approvalStatus == ApprovalStatus.approved)
                .Include(m => m.User).Include(m => m.WorkingHours)
                .ToListAsync();
        }
        public override async Task<MedicalTestProviderProfile?> GetByIdAsync(int id)
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User).Include(m => m.WorkingHours)
                .FirstOrDefaultAsync(m => m.Id == id && m.approvalStatus == ApprovalStatus.approved);
        }
        public async Task<MedicalTestProviderProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.approvalStatus == ApprovalStatus.approved);
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null)
        {
            var query = _context.MedicalTestProviders
                .Where(m => m.approvalStatus == ApprovalStatus.approved);
            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }
            query = query.Include(m => m.User);
            return await query.OrderByDescending(M => M.Avg_Rating).ToListAsync();
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetProfilesByTypeAsync(MedicalTestProviderType type)
        {
            var profiles = await _context.MedicalTestProviders
                .Where(m => m.Type == type)
                .Include(m => m.User)
                .ToListAsync();
            return profiles;
        }

        public async Task<(IEnumerable<MedicalTestProviderProfile> Items, int TotalCount)> SearchAsync(string? searchTerm, MedicalTestProviderType? type, int skip, int take)
        {
            var query = _context.MedicalTestProviders
                .Where(m => m.approvalStatus == ApprovalStatus.approved);

            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Name.Contains(searchTerm));
            }

            var candidates = await query.Include(m => m.User).ToListAsync();

            IEnumerable<MedicalTestProviderProfile> filteredProviders;

            const double similarityThreshold = 0.5;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredProviders = candidates
                    .Where(m => FuzzyMatcher.SimilarityScore(m.Name, searchTerm) >= similarityThreshold)
                    .ToList();
            }
            else
            {
                filteredProviders = candidates;
            }
            var totalCount = filteredProviders.Count();

            var pagedItems = filteredProviders
                .OrderByDescending(m => m.Avg_Rating)
                .Skip(skip)
                .Take(take)
                .ToList();

            return (pagedItems, totalCount);
        }
    }
}