namespace Dactra.Services.Implementation
{
    public class QuestionService: IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IHubContext<QuestionsHub> _hub;
        private readonly ApplicationDbContext _context;

        public QuestionService(
            IQuestionRepository questionRepo,
            IAnswerRepository answerRepo,
            IHubContext<QuestionsHub> hub,
            ApplicationDbContext context)
        {
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _hub = hub;
            _context = context;
        }

        public async Task CreateQuestionAsync(int patientId, CreateQuestionDto dto)
        {
            var question = new Questions
            {
                Text = dto.Text,
                MajorId = dto.MajorId,
                PatientId = patientId
            };

            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveAsync();

          
            await _hub.Clients.All.SendAsync("NewQuestion", question);
        }
        public async Task UpdateQuestionAsync(int questionId, int patientId, CreateQuestionDto dto)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException("Question not found");

            if (question.PatientId != patientId)
                throw new UnauthorizedAccessException();

            question.Text = dto.Text;
            question.MajorId = dto.MajorId;

            _questionRepo.Update(question);
            await _questionRepo.SaveAsync();

            await _hub.Clients.All.SendAsync("UpdatedQuestion", question);
        }
        public async Task DeleteQuestionAsync(int questionId, int patientId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException("Question not found");

            if (question.PatientId != patientId)
                throw new UnauthorizedAccessException();

            question.isDeleted = true;
            _questionRepo.Update(question);
            await _questionRepo.SaveAsync();

            await _hub.Clients.All.SendAsync("DeletedQuestion", questionId);
        }

        public async Task AnswerQuestionAsync(int questionId, int doctorId, CreateAnswerDto dto)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw new KeyNotFoundException("Question not found");

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null || doctor.SpecializationId != question.MajorId)
                throw new UnauthorizedAccessException("You cannot answer this question");

            var answer = new Answer
            {
                QuestionId = questionId,
                DoctorId = doctorId,
                Content = dto.Content
            };

            await _answerRepo.AddAsync(answer);
            await _answerRepo.SaveAsync();

            await _hub.Clients.All.SendAsync("NewAnswer", answer);
        
         }
    }
}
