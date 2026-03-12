namespace Dactra.Repositories.Implementation
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly ApplicationDbContext _context;
        public PostLikeRepository(ApplicationDbContext context) => _context = context;

        public async Task<PostLike?> GetAsync(int postId, string userId)
            => await _context.PostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        public async Task<int> GetCountByPostIdAsync(int postId)
            => await _context.PostLikes.CountAsync(l => l.PostId == postId);

        public async Task<bool> IsLikedByUserAsync(int postId, string userId)
            => await _context.PostLikes.AnyAsync(l => l.PostId == postId && l.UserId == userId);

        public async Task<PostLike> AddAsync(PostLike like)
        {
            _context.PostLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task RemoveAsync(int postId, string userId)
        {
            var like = await GetAsync(postId, userId);
            if (like != null)
            {
                _context.PostLikes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
    }
}
