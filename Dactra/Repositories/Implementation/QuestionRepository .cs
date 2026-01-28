namespace Dactra.Repositories.Implementation
{
    public class QuestionRepository: IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Questions>> GetAllAsync()
        {
            return await _context.Questions
                .Where(q => !q.isDeleted)
                .Include(q => q.Patient)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.Doctor)
                .ToListAsync();
        }

        public async Task<Questions?> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id && !q.isDeleted);
        }

        public async Task AddAsync(Questions question)
        {
            await _context.Questions.AddAsync(question);
        }

        public void Update(Questions question)
        {
            _context.Questions.Update(question);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
