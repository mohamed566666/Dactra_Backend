namespace Dactra.Repositories.Implementation
{
    public class QuestionInterestRepository : IQuestionInterestRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionInterestRepository(ApplicationDbContext context) => _context = context;

        public async Task<QuestionInterest?> GetAsync(int questionId, string userId)
            => await _context.QuestionInterests
                .FirstOrDefaultAsync(i => i.QuestionId == questionId && i.UserId == userId);

        public async Task<int> GetCountByQuestionIdAsync(int questionId)
            => await _context.QuestionInterests
                .CountAsync(i => i.QuestionId == questionId && !i.Question.isDeleted);

        public async Task<bool> IsInterestedByUserAsync(int questionId, string userId)
            => await _context.QuestionInterests
                .AnyAsync(i => i.QuestionId == questionId && i.UserId == userId);

        public async Task<List<string>> GetInterestedUserIdsByQuestionIdAsync(int questionId)
            => await _context.QuestionInterests
                .Where(i => i.QuestionId == questionId)
                .Select(i => i.UserId)
                .ToListAsync();

        public async Task<QuestionInterest> AddAsync(QuestionInterest interest)
        {
            _context.QuestionInterests.Add(interest);
            await _context.SaveChangesAsync();
            return interest;
        }

        public async Task RemoveAsync(int questionId, string userId)
        {
            var interest = await GetAsync(questionId, userId);
            if (interest != null)
            {
                _context.QuestionInterests.Remove(interest);
                await _context.SaveChangesAsync();
            }
        }
    }
}
