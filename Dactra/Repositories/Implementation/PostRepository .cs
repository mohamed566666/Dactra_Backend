using Dactra.DTOs.PostDTOs;

namespace Dactra.Repositories.Implementation
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Posts.AsQueryable();
            if (!includeDeleted)
                query = query.Where(p => !p.isDeleted);
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetByIdWithDetailsAsync(int id, string? currentUserId = null)
        {
            return await _context.Posts
                .Where(p => p.Id == id && !p.isDeleted)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                        .ThenInclude(r => r.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .Include(p => p.PostTags)
                    .ThenInclude(pt => pt.Tag)
                .Include(p => p.SavedBy)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, string? currentUserId = null)
        {
            var query = _context.Posts
                .Where(p => !p.isDeleted)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.SavedBy)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetByDoctorIdAsync(int doctorId, int page, int pageSize)
        {
            var query = _context.Posts
                .Where(p => p.DoctorId == doctorId && !p.isDeleted)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetByTagAsync(int tagId, int page, int pageSize)
        {
            var query = _context.Posts
                .Where(p => !p.isDeleted && p.PostTags.Any(pt => pt.TagId == tagId))
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.isDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Posts.AnyAsync(p => p.Id == id && !p.isDeleted);

        public async Task<bool> BelongsToDoctorAsync(int postId, int doctorId)
            => await _context.Posts.AnyAsync(p => p.Id == postId && p.DoctorId == doctorId && !p.isDeleted);

        public async Task<(List<Post> Posts, int TotalCount)> GetFilteredAsync(PostFilterDto filter, string userId, int page, int pageSize)
        {
            var query = _context.Posts
                .Where(p => !p.isDeleted)
                .Include(p => p.Doctor)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.SavedBy)
                .Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .AsQueryable();

            query = filter switch
            {
                PostFilterDto.Liked => query.Where(p => p.Likes.Any(l => l.UserId == userId)),
                PostFilterDto.Saved => query.Where(p => p.SavedBy.Any(s => s.UserId == userId)),
                PostFilterDto.Commented => query.Where(p => p.Comments.Any(c => c.UserId == userId)),
                _ => query
            };

            query = query.OrderByDescending(p => p.CreatedAt);

            var total = await query.CountAsync();
            var posts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (posts, total);
        }

        public async Task<UserPostStatsDto> GetUserStatsAsync(string userId)
        {
            var liked = await _context.PostLikes.CountAsync(l => l.UserId == userId);
            var saved = await _context.SavedPosts.CountAsync(s => s.UserId == userId);
            var commented = await _context.comments
                                .Where(c => c.UserId == userId)
                                .Select(c => c.PostId)
                                .Distinct()
                                .CountAsync();

            return new UserPostStatsDto
            {
                TotalLiked = liked,
                TotalSaved = saved,
                TotalCommented = commented
            };
        }
    }
}
