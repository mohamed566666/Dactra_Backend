namespace Dactra.Repositories.Implementation
{
    public class QuestionAnswerRepository: IQuestionAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        public QuestionAnswerRepository(ApplicationDbContext context) => _context = context;

        public async Task<QuestionAnswer?> GetByIdAsync(int id)
        {
            return await _context.QuestionAnswers
                .Where(a => !a.isDeleted)
                .Include(a => a.Doctor).ThenInclude(d => d.specialization)
                .Include(a => a.Replies.Where(r => !r.isDeleted))
                    .ThenInclude(r => r.Doctor).ThenInclude(d => d.specialization)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<QuestionAnswer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.QuestionAnswers
                .Where(a => a.QuestionId == questionId && !a.isDeleted && a.ParentAnswerId == null)
                .Include(a => a.Doctor).ThenInclude(d => d.specialization)
                .Include(a => a.Replies.Where(r => !r.isDeleted))
                    .ThenInclude(r => r.Doctor).ThenInclude(d => d.specialization)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuestionAnswer> CreateAsync(QuestionAnswer answer)
        {
            _context.QuestionAnswers.Add(answer);
            await _context.SaveChangesAsync();
            await _context.Entry(answer).Reference(a => a.Doctor).LoadAsync();
            await _context.Entry(answer.Doctor).Reference(d => d.specialization).LoadAsync();
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

        public async Task<bool> BelongsToDoctorAsync(int answerId, int doctorId)
            => await _context.QuestionAnswers.AnyAsync(a => a.Id == answerId && a.DoctorId == doctorId && !a.isDeleted);

        public async Task<int> GetActiveCountByQuestionIdAsync(int questionId)
            => await _context.QuestionAnswers.CountAsync(a => a.QuestionId == questionId && !a.isDeleted);
    }
}
