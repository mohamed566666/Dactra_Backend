namespace Dactra.Repositories.Implementation
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentLikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsLikedByUserAsync(int commentId, string userId)
            => await _context.CommentLikes.AnyAsync(cl =>
                cl.CommentId == commentId && cl.UserId == userId);

        public async Task<CommentLike?> GetLikeAsync(int commentId, string userId)
            => await _context.CommentLikes
                .FirstOrDefaultAsync(cl =>
                    cl.CommentId == commentId && cl.UserId == userId);

        public async Task<CommentLike> AddLikeAsync(int commentId, string userId)
        {
            var like = new CommentLike
            {
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.CommentLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> RemoveLikeAsync(int commentId, string userId)
        {
            var like = await GetLikeAsync(commentId, userId);
            if (like == null) return false;

            _context.CommentLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetLikesCountAsync(int commentId)
            => await _context.CommentLikes.CountAsync(cl => cl.CommentId == commentId);
    }
}
