namespace Dactra.Repositories.Implementation
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Tag>> GetAllAsync()
            => await _context.Tags.OrderBy(t => t.Name).ToListAsync();

        public async Task<Tag?> GetByIdAsync(int id)
            => await _context.Tags.FindAsync(id);

        public async Task<Tag?> GetByNameAsync(string name)
            => await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

        public async Task<List<Tag>> GetByIdsAsync(List<int> ids)
            => await _context.Tags.Where(t => ids.Contains(t.Id)).ToListAsync();

        public async Task<List<Tag>> GetByNamesAsync(List<string> names)
        {
            var lowerNames = names.Select(n => n.ToLower()).ToList();
            return await _context.Tags
                .Where(t => lowerNames.Contains(t.Name.ToLower()))
                .ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
