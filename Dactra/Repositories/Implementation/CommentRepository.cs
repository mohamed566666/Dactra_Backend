namespace Dactra.Repositories.Implementation
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.comments
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Comment>> GetByPostIdAsync(int postId)
        {
            return await _context.comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Likes)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Likes)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }


        public async Task<Comment> CreateAsync(Comment comment)
        {
            _context.comments.Add(comment);
            await _context.SaveChangesAsync();
            await _context.Entry(comment).Reference(c => c.User).LoadAsync();
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            _context.comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.comments.FindAsync(id);
            if (comment != null)
            {
                _context.comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.comments.AnyAsync(c => c.Id == id);

        public async Task<bool> BelongsToUserAsync(int commentId, string userId)
            => await _context.comments.AnyAsync(c => c.Id == commentId && c.UserId == userId);

        public async Task<int> GetCountByPostIdAsync(int postId)
            => await _context.comments.CountAsync(c => c.PostId == postId);
    }
}