namespace Dactra.Repositories.Implementation
{
    public class SavedPostRepository : ISavedPostRepository
    {
        private readonly ApplicationDbContext _context;
        public SavedPostRepository(ApplicationDbContext context) => _context = context;

        public async Task<SavedPost?> GetAsync(int postId, string userId)
            => await _context.SavedPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PostId == postId && s.UserId == userId);

        public async Task<(List<SavedPost> Items, int TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize)
        {
            var query = _context.SavedPosts
                .Where(s => s.UserId == userId)
                .Include(s => s.Post).ThenInclude(p => p.Doctor)
                .Include(s => s.Post).ThenInclude(p => p.Likes)
                .Include(s => s.Post).ThenInclude(p => p.Comments)
                .Include(s => s.Post).ThenInclude(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(s => s.SavedAt);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
            return (items, total);
        }

        public async Task<bool> IsSavedByUserAsync(int postId, string userId)
            => await _context.SavedPosts.AnyAsync(s => s.PostId == postId && s.UserId == userId);

        public async Task<SavedPost> AddAsync(SavedPost savedPost)
        {
            _context.SavedPosts.Add(savedPost);
            await _context.SaveChangesAsync();
            return savedPost;
        }

        public async Task RemoveAsync(int postId, string userId)
        {
            var saved = await GetAsync(postId, userId);
            if (saved != null)
            {
                _context.SavedPosts.Remove(saved);
                await _context.SaveChangesAsync();
            }
        }
    }
}