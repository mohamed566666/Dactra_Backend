namespace Dactra.Repositories.Implementation
{
    public class QuestionAnswerRepository : IQuestionAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionAnswerRepository(ApplicationDbContext context) => _context = context;

        public async Task<(List<QuestionAnswer> Answers, int TotalCount)> GetTopLevelAnswersByQuestionIdAsync(
            int questionId, int page, int pageSize)
        {
            var query = _context.QuestionAnswers
                .Where(a => a.QuestionId == questionId
                         && !a.isDeleted
                         && a.ParentAnswerId == null)
                .Include(a => a.Answerer)
                .Include(a => a.Likes)
                .OrderBy(a => a.CreatedAt);

            var total = await query.CountAsync();

            var answers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (answers, total);
        }

        public async Task<(List<QuestionAnswer> Replies, int TotalCount)> GetRepliesByParentAnswerIdAsync(
            int parentAnswerId, int page, int pageSize)
        {
            var query = _context.QuestionAnswers
                .Where(a => a.ParentAnswerId == parentAnswerId && !a.isDeleted)
                .Include(a => a.Answerer)
                .Include(a => a.Likes)
                .OrderBy(a => a.CreatedAt);

            var total = await query.CountAsync();

            var replies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (replies, total);
        }

        public async Task<int> GetRepliesCountAsync(int parentAnswerId)
            => await _context.QuestionAnswers
                .CountAsync(a => a.ParentAnswerId == parentAnswerId && !a.isDeleted);

        public async Task<QuestionAnswer?> GetByIdAsync(int id)
        {
            return await _context.QuestionAnswers
                .Where(a => !a.isDeleted)
                .Include(a => a.Answerer)
                .Include(a => a.Likes)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<QuestionAnswer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.QuestionAnswers
                .Where(a => a.QuestionId == questionId && !a.isDeleted && a.ParentAnswerId == null)
                .Include(a => a.Answerer)
                .Include(a => a.Likes)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionAnswer> CreateAsync(QuestionAnswer answer)
        {
            _context.QuestionAnswers.Add(answer);
            await _context.SaveChangesAsync();
            await _context.Entry(answer).Reference(a => a.Answerer).LoadAsync();
            return answer;
        }

        public async Task<QuestionAnswer> UpdateAsync(QuestionAnswer answer)
        {
            _context.QuestionAnswers.Update(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var answer = await _context.QuestionAnswers.FindAsync(id);
            if (answer != null)
            {
                answer.isDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.QuestionAnswers.AnyAsync(a => a.Id == id && !a.isDeleted);

        public async Task<bool> BelongsToUserAsync(int answerId, string userId)
            => await _context.QuestionAnswers.AnyAsync(a => a.Id == answerId && a.AnswererUserId == userId && !a.isDeleted);

        public async Task<Question?> GetQuestionByAnswerIdAsync(int answerId)
            => await _context.QuestionAnswers
                .Where(a => a.Id == answerId && !a.isDeleted)
                .Include(a => a.Question)
                    .ThenInclude(q => q.Patient)
                .Select(a => a.Question)
                .FirstOrDefaultAsync();
    }
}