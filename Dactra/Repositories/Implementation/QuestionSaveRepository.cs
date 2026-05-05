namespace Dactra.Repositories.Implementation
{
    public class QuestionSaveRepository : IQuestionSaveRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionSaveRepository(ApplicationDbContext context) => _context = context;

        public async Task<QuestionSave?> GetAsync(int questionId, string userId)
            => await _context.QuestionSaves
                .FirstOrDefaultAsync(s => s.QuestionId == questionId && s.UserId == userId);

        public async Task<(List<QuestionSave> Items, int TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize)
        {
            var query = _context.QuestionSaves
                .Where(s => s.UserId == userId && !s.Question.isDeleted)
                .Include(s => s.Question).ThenInclude(q => q.Answers.Where(a => !a.isDeleted))
                .Include(s => s.Question).ThenInclude(q => q.Interests)
                .Include(s => s.Question).ThenInclude(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
                .OrderByDescending(s => s.SavedAt);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<bool> IsSavedByUserAsync(int questionId, string userId)
            => await _context.QuestionSaves
                .AnyAsync(s => s.QuestionId == questionId && s.UserId == userId);

        public async Task<QuestionSave> AddAsync(QuestionSave save)
        {
            _context.QuestionSaves.Add(save);
            await _context.SaveChangesAsync();
            return save;
        }

        public async Task RemoveAsync(int questionId, string userId)
        {
            var save = await GetAsync(questionId, userId);
            if (save != null)
            {
                _context.QuestionSaves.Remove(save);
                await _context.SaveChangesAsync();
            }
        }
    }
}