using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Implementation
{
    public class QuestionAnswerService : IQuestionAnswerService
    {
        private readonly IQuestionAnswerRepository _answerRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IQuestionInterestRepository _interestRepo;
        private readonly IHubContext<QuestionHub> _hub;

        public QuestionAnswerService(
            IQuestionAnswerRepository answerRepo,
            IQuestionRepository questionRepo,
            IQuestionInterestRepository interestRepo,
            IHubContext<QuestionHub> hub)
        {
            _answerRepo = answerRepo;
            _questionRepo = questionRepo;
            _interestRepo = interestRepo;
            _hub = hub;
        }

        public async Task<List<AnswerResponseDto>> GetByQuestionIdAsync(int questionId)
        {
            if (!await _questionRepo.ExistsAsync(questionId))
                throw new KeyNotFoundException($"Question {questionId} not found.");

            var answers = await _answerRepo.GetByQuestionIdAsync(questionId);
            return answers.Select(MapToDto).ToList();
        }

        public async Task<AnswerResponseDto> CreateAsync(int questionId, CreateAnswerDto dto, int doctorId)
        {
            if (!await _questionRepo.ExistsAsync(questionId))
                throw new KeyNotFoundException($"Question {questionId} not found.");

            if (dto.ParentAnswerId.HasValue && !await _answerRepo.ExistsAsync(dto.ParentAnswerId.Value))
                throw new KeyNotFoundException($"Parent answer {dto.ParentAnswerId} not found.");

            var answer = new QuestionAnswer
            {
                Content = dto.Content,
                QuestionId = questionId,
                DoctorId = doctorId,
                ParentAnswerId = dto.ParentAnswerId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _answerRepo.CreateAsync(answer);
            var responseDto = MapToDto(created);

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(questionId))
                .SendAsync("AnswerAdded", responseDto);

            var interestedUserIds = await _interestRepo.GetInterestedUserIdsByQuestionIdAsync(questionId);
            foreach (var userId in interestedUserIds)
            {
                await _hub.Clients.Group(QuestionHub.UserGroup(userId))
                    .SendAsync("NewAnswerNotification", new
                    {
                        questionId,
                        answerId = created.Id,
                        doctorName = created.Doctor == null ? "" :
                            created.Doctor.FirstName + " " + created.Doctor.LastName,
                        message = "A doctor answered a question you're interested in."
                    });
            }

            return responseDto;
        }

        public async Task<AnswerResponseDto> UpdateAsync(int answerId, UpdateAnswerDto dto, int doctorId)
        {
            if (!await _answerRepo.BelongsToDoctorAsync(answerId, doctorId))
                throw new UnauthorizedAccessException("You can only edit your own answers.");

            var answer = await _answerRepo.GetByIdAsync(answerId)
                ?? throw new KeyNotFoundException($"Answer {answerId} not found.");

            answer.Content = dto.Content;
            answer.UpdatedAt = DateTime.UtcNow;

            var updated = await _answerRepo.UpdateAsync(answer);
            var responseDto = MapToDto(updated);

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(answer.QuestionId))
                .SendAsync("AnswerUpdated", responseDto);

            return responseDto;
        }

        public async Task DeleteAsync(int answerId, int doctorId)
        {
            if (!await _answerRepo.BelongsToDoctorAsync(answerId, doctorId))
                throw new UnauthorizedAccessException("You can only delete your own answers.");

            var answer = await _answerRepo.GetByIdAsync(answerId)
                ?? throw new KeyNotFoundException($"Answer {answerId} not found.");

            int questionId = answer.QuestionId;
            await _answerRepo.SoftDeleteAsync(answerId);

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(questionId))
                .SendAsync("AnswerDeleted", new { answerId, questionId });
        }

        private static AnswerResponseDto MapToDto(QuestionAnswer a) => new()
        {
            Id = a.Id,
            Content = a.Content,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            QuestionId = a.QuestionId,
            ParentAnswerId = a.ParentAnswerId,
            Doctor = a.Doctor == null ? null! : new DoctorAnswerSummaryDto
            {
                Id = a.Doctor.Id,
                FullName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                Specialty = a.Doctor.specialization?.Name,
                ProfileImageUrl = null
            },
            Replies = a.Replies?
                .Where(r => !r.isDeleted)
                .Select(MapToDto)
                .ToList() ?? new List<AnswerResponseDto>()
        };
    }
}
