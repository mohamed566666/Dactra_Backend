namespace Dactra.Repositories.Implementation
{
    public class QuestionAnswerLikeRepository : IQuestionAnswerLikeRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionAnswerLikeRepository(ApplicationDbContext context) => _context = context;

        public async Task<bool> IsLikedByUserAsync(int answerId, string userId)
            => await _context.QuestionAnswerLikes.AnyAsync(l =>
                l.AnswerId == answerId && l.UserId == userId);

        public async Task<QuestionAnswerLike?> GetLikeAsync(int answerId, string userId)
            => await _context.QuestionAnswerLikes
                .FirstOrDefaultAsync(l => l.AnswerId == answerId && l.UserId == userId);

        public async Task<QuestionAnswerLike> AddLikeAsync(int answerId, string userId)
        {
            var like = new QuestionAnswerLike
            {
                AnswerId = answerId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.QuestionAnswerLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> RemoveLikeAsync(int answerId, string userId)
        {
            var like = await GetLikeAsync(answerId, userId);
            if (like == null) return false;
            _context.QuestionAnswerLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetLikesCountAsync(int answerId)
            => await _context.QuestionAnswerLikes.CountAsync(l => l.AnswerId == answerId);
    }
}
