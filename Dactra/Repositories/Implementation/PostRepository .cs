namespace Dactra.Repositories.Implementation
{
    public class PostRepository: IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Where(p => !p.isDeleted)
                .Include(p => p.Category)
                .Include(p => p.Doctor)
                .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
        }

        public void Update(Post post)
        {
            _context.Posts.Update(post);
        }

        public void SoftDelete(Post post)
        {
            post.isDeleted = true;
            _context.Posts.Update(post);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
