namespace Dactra.Repositories.Implementation
{
    public class PostTagRepository : IPostTagRepository
    {
        private readonly ApplicationDbContext _context;
        public PostTagRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<PostTag>> GetByPostIdAsync(int postId)
            => await _context.PostTags.Where(pt => pt.PostId == postId).Include(pt => pt.Tag).ToListAsync();

        public async Task AddRangeAsync(List<PostTag> postTags)
        {
            _context.PostTags.AddRange(postTags);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveByPostIdAsync(int postId)
        {
            var postTags = await _context.PostTags.Where(pt => pt.PostId == postId).ToListAsync();
            _context.PostTags.RemoveRange(postTags);
            await _context.SaveChangesAsync();
        }

        public async Task ReplaceTagsAsync(int postId, List<int> newTagIds)
        {
            var existing = await _context.PostTags.Where(pt => pt.PostId == postId).ToListAsync();
            _context.PostTags.RemoveRange(existing);

            var newPostTags = newTagIds.Select(tagId => new PostTag { PostId = postId, TagId = tagId }).ToList();
            _context.PostTags.AddRange(newPostTags);

            await _context.SaveChangesAsync();
        }
    }
}
