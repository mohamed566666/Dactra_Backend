using Dactra.DTOs.QuestionDTOs;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class QuestionAnswerService : IQuestionAnswerService
    {
        private readonly IQuestionAnswerRepository _answerRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IQuestionInterestRepository _interestRepo;
        private readonly IDoctorProfileRepository _doctorRepo;
        private readonly IHubContext<QuestionHub> _hub;

        public QuestionAnswerService(
            IQuestionAnswerRepository answerRepo,
            IQuestionRepository questionRepo,
            IQuestionInterestRepository interestRepo,
            IDoctorProfileRepository doctorRepo,
            IHubContext<QuestionHub> hub)
        {
            _answerRepo = answerRepo;
            _questionRepo = questionRepo;
            _interestRepo = interestRepo;
            _doctorRepo = doctorRepo;
            _hub = hub;
        }

        public async Task<PagedResultDto<AnswerResponseDto>> GetTopLevelAnswersByQuestionIdAsync(
            int questionId, int page, int pageSize, string? currentUserId = null)
        {
            if (!await _questionRepo.ExistsAsync(questionId))
                throw new KeyNotFoundException($"Question {questionId} not found.");

            var (answers, total) = await _answerRepo.GetTopLevelAnswersByQuestionIdAsync(questionId, page, pageSize);

            var answererIds = answers.Select(a => a.AnswererUserId).Distinct().ToList();
            var doctorProfiles = new Dictionary<string, DoctorProfile?>();
            foreach (var uid in answererIds)
            {
                doctorProfiles[uid] = await _doctorRepo.GetByUserIdAsync(uid);
            }

            var items = new List<AnswerResponseDto>();
            foreach (var a in answers)
            {
                var dto = MapToDto(a, currentUserId, doctorProfiles);
                dto.RepliesCount = await _answerRepo.GetRepliesCountAsync(a.Id);
                items.Add(dto);
            }

            return new PagedResultDto<AnswerResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<AnswerResponseDto>> GetRepliesByParentAnswerIdAsync(
            int parentAnswerId, int page, int pageSize, string? currentUserId = null)
        {
            var parent = await _answerRepo.GetByIdAsync(parentAnswerId)
                ?? throw new KeyNotFoundException($"Parent answer {parentAnswerId} not found.");

            var (replies, total) = await _answerRepo.GetRepliesByParentAnswerIdAsync(parentAnswerId, page, pageSize);

            var answererIds = replies.Select(a => a.AnswererUserId).Distinct().ToList();
            var doctorProfiles = new Dictionary<string, DoctorProfile?>();
            foreach (var uid in answererIds)
            {
                doctorProfiles[uid] = await _doctorRepo.GetByUserIdAsync(uid);
            }

            var items = replies.Select(r => MapToDto(r, currentUserId, doctorProfiles)).ToList();

            return new PagedResultDto<AnswerResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<AnswerResponseDto> CreateAsync(int questionId, CreateAnswerDto dto, string userId, bool isDoctor)
        {
            var question = await _questionRepo.GetByIdAsync(questionId)
                ?? throw new KeyNotFoundException($"Question {questionId} not found.");

            var isQuestionOwner = question.Patient?.UserId == userId;
            if (!isDoctor && !isQuestionOwner)
                throw new UnauthorizedAccessException("Only doctors or the question owner can answer.");

            if (dto.ParentAnswerId.HasValue && !await _answerRepo.ExistsAsync(dto.ParentAnswerId.Value))
                throw new KeyNotFoundException($"Parent answer {dto.ParentAnswerId} not found.");

            var answer = new QuestionAnswer
            {
                Content = dto.Content,
                QuestionId = questionId,
                AnswererUserId = userId,
                ParentAnswerId = dto.ParentAnswerId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var created = await _answerRepo.CreateAsync(answer);
            var doctorProfile = isDoctor ? await _doctorRepo.GetByUserIdAsync(userId) : null;
            var responseDto = MapToDto(created, userId, new() { [userId] = doctorProfile });

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(questionId))
                .SendAsync("AnswerAdded", responseDto);

            if (!dto.ParentAnswerId.HasValue)
            {
                var interested = await _interestRepo.GetInterestedUserIdsByQuestionIdAsync(questionId);
                foreach (var uId in interested.Where(u => u != userId))
                {
                    await _hub.Clients.Group(QuestionHub.UserGroup(uId))
                        .SendAsync("NewAnswerNotification", new { questionId, answerId = created.Id });
                }
            }

            return responseDto;
        }

        public async Task<AnswerResponseDto> UpdateAsync(int answerId, UpdateAnswerDto dto, string userId, bool isDoctor)
        {
            var answer = await _answerRepo.GetByIdAsync(answerId)
                ?? throw new KeyNotFoundException($"Answer {answerId} not found.");

            if (answer.AnswererUserId != userId && !isDoctor)
                throw new UnauthorizedAccessException("You can only edit your own answers.");

            answer.Content = dto.Content;
            answer.UpdatedAt = DateTimeOffset.UtcNow;
            var updated = await _answerRepo.UpdateAsync(answer);

            var doctorProfile = await _doctorRepo.GetByUserIdAsync(updated.AnswererUserId);
            var responseDto = MapToDto(updated, userId, new() { [updated.AnswererUserId] = doctorProfile });

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(answer.QuestionId))
                .SendAsync("AnswerUpdated", responseDto);

            return responseDto;
        }

        public async Task DeleteAsync(int answerId, string userId, bool isDoctor)
        {
            var answer = await _answerRepo.GetByIdAsync(answerId)
                ?? throw new KeyNotFoundException($"Answer {answerId} not found.");

            if (answer.AnswererUserId != userId && !isDoctor)
                throw new UnauthorizedAccessException("You can only delete your own answers.");

            await _answerRepo.SoftDeleteAsync(answerId);
            await _hub.Clients.Group(QuestionHub.QuestionGroupName(answer.QuestionId))
                .SendAsync("AnswerDeleted", new { answerId, questionId = answer.QuestionId });
        }
        private static AnswerResponseDto MapToDto(
            QuestionAnswer a,
            string? currentUserId,
            Dictionary<string, DoctorProfile?> doctorProfiles)
        {
            var doctorProfile = doctorProfiles.GetValueOrDefault(a.AnswererUserId);
            var isDoctor = doctorProfile != null;

            return new AnswerResponseDto
            {
                Id = a.Id,
                Content = a.Content,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                QuestionId = a.QuestionId,
                ParentAnswerId = a.ParentAnswerId,

                Answerer = new AnswererInfoDto
                {
                    FullName = isDoctor
                        ? $"{doctorProfile!.FirstName} {doctorProfile.LastName}"
                        : (a.Answerer?.UserName ?? "user"),
                    ProfileImageUrl = null,
                    IsDoctor = isDoctor,
                    Specialty = isDoctor ? doctorProfile?.specialization?.Name : null,
                    YearsOfExperience = isDoctor ? doctorProfile?.YearsOfExperience : null
                },

                LikesCount = a.Likes?.Count ?? 0,
                IsLikedByCurrentUser = currentUserId != null &&
                    a.Likes?.Any(l => l.UserId == currentUserId) == true,

                RepliesCount = 0
            };
        }
    }
}
