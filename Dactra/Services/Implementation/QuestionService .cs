using Dactra.DTOs.QuestionDTOs;
using Dactra.DTOs.TagDTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Dactra.Services.Implementation
{
    public class QuestionService: IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly IQuestionInterestRepository _interestRepo;
        private readonly IQuestionSaveRepository _saveRepo;
        private readonly ITagRepository _tagRepo;
        private readonly IAITaggingService _aiTagging;
        private readonly IHubContext<QuestionHub> _hub;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            IQuestionRepository questionRepo,
            IQuestionInterestRepository interestRepo,
            IQuestionSaveRepository saveRepo,
            ITagRepository tagRepo,
            IAITaggingService aiTagging,
            IHubContext<QuestionHub> hub,
            ILogger<QuestionService> logger)
        {
            _questionRepo = questionRepo;
            _interestRepo = interestRepo;
            _saveRepo = saveRepo;
            _tagRepo = tagRepo;
            _aiTagging = aiTagging;
            _hub = hub;
            _logger = logger;
        }

        public async Task<QuestionFeedResponseDto> GetAllAsync(int page, int pageSize, string? currentUserId = null)
        {
            var (questions, total) = await _questionRepo.GetAllAsync(page, pageSize);
            var items = new List<QuestionResponseDto>();
            foreach (var q in questions)
                items.Add(await MapToResponseDtoAsync(q, currentUserId));

            var stats = currentUserId != null
                ? await _questionRepo.GetUserQuestionStatsAsync(currentUserId)
                : new UserQuestionStatsDto();

            return new QuestionFeedResponseDto
            {
                Questions = new PagedResultDto<QuestionResponseDto>
                {
                    Items = items,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize
                },
                Stats = stats
            };
        }

        public async Task<QuestionResponseDto> GetByIdAsync(int id, string? currentUserId = null)
        {
            var question = await _questionRepo.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException($"Question {id} not found.");
            return await MapToResponseDtoAsync(question, currentUserId);
        }

        public async Task<PagedResultDto<QuestionResponseDto>> GetByPatientIdAsync(int patientId, int page, int pageSize)
        {
            var (questions, total) = await _questionRepo.GetByPatientIdAsync(patientId, page, pageSize);
            var items = new List<QuestionResponseDto>();
            foreach (var q in questions)
                items.Add(await MapToResponseDtoAsync(q, null));

            return new PagedResultDto<QuestionResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }


        public async Task<PagedResultDto<QuestionResponseDto>> GetByTagAsync(int tagId, int page, int pageSize)
        {
            var (questions, total) = await _questionRepo.GetByTagAsync(tagId, page, pageSize);
            var items = new List<QuestionResponseDto>();
            foreach (var q in questions)
                items.Add(await MapToResponseDtoAsync(q, null));

            return new PagedResultDto<QuestionResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<QuestionResponseDto>> GetMyFilteredQuestionsAsync(
            QuestionFilterDto filter, string userId, int page, int pageSize)
        {
            var (questions, total) = await _questionRepo.GetFilteredAsync(filter, userId, page, pageSize);
            var items = new List<QuestionResponseDto>();
            foreach (var q in questions)
                items.Add(await MapToResponseDtoAsync(q, userId));

            return new PagedResultDto<QuestionResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<QuestionResponseDto>> GetMyQuestionsAsync(int patientId, int page, int pageSize)
            => await GetByPatientIdAsync(patientId, page, pageSize);

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto, int patientId)
        {
            var question = new Question
            {
                Content = dto.Content,
                PatientId = patientId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var created = await _questionRepo.CreateAsync(question);

            await AssignTagsAsync(created.Id, dto.Content);

            var fullQuestion = await _questionRepo.GetByIdWithDetailsAsync(created.Id)!;
            var responseDto = await MapToResponseDtoAsync(fullQuestion!, null);

            await _hub.Clients.Group(QuestionHub.GlobalFeedGroup())
                .SendAsync("QuestionCreated", responseDto);

            return responseDto;
        }

        public async Task<QuestionResponseDto> UpdateAsync(int id, UpdateQuestionDto dto, int patientId)
        {
            if (!await _questionRepo.BelongsToPatientAsync(id, patientId))
                throw new UnauthorizedAccessException("You can only edit your own questions.");

            var question = await _questionRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Question {id} not found.");

            question.Content = dto.Content;
            question.UpdatedAt = DateTimeOffset.UtcNow;

            await _questionRepo.UpdateAsync(question);

            await AssignTagsAsync(id, dto.Content);

            var fullQuestion = await _questionRepo.GetByIdWithDetailsAsync(id)!;
            var responseDto = await MapToResponseDtoAsync(fullQuestion!, null);

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(id))
                .SendAsync("QuestionUpdated", responseDto);
            await _hub.Clients.Group(QuestionHub.GlobalFeedGroup())
                .SendAsync("QuestionUpdated", responseDto);

            return responseDto;
        }

        public async Task DeleteAsync(int id, int patientId)
        {
            if (!await _questionRepo.BelongsToPatientAsync(id, patientId))
                throw new UnauthorizedAccessException("You can only delete your own questions.");

            await _questionRepo.SoftDeleteAsync(id);

            await _hub.Clients.Group(QuestionHub.QuestionGroupName(id))
                .SendAsync("QuestionDeleted", new { questionId = id });
            await _hub.Clients.Group(QuestionHub.GlobalFeedGroup())
                .SendAsync("QuestionDeleted", new { questionId = id });
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task AssignTagsAsync(int questionId, string content)
        {
            try
            {
                var allTags = await _tagRepo.GetAllAsync();
                var availableTagNames = allTags.Select(t => t.Name).ToList();

                var matchedNames = await _aiTagging.ExtractTagsFromContentAsync(content, availableTagNames);
                var matchedTags = await _tagRepo.GetByNamesAsync(matchedNames);

                await _questionRepo.AssignTagsAsync(questionId, matchedTags.Select(t => t.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-tagging failed for question {QuestionId}", questionId);
            }
        }

        private async Task<QuestionResponseDto> MapToResponseDtoAsync(Question q, string? currentUserId)
        {
            bool isInterested = currentUserId != null && await _interestRepo.IsInterestedByUserAsync(q.Id, currentUserId);
            bool isSaved = currentUserId != null && await _saveRepo.IsSavedByUserAsync(q.Id, currentUserId);

            QuestionStatsDto? Stats = await _questionRepo.GetQuestionStatsAsync(q.Id);

            return new QuestionResponseDto
            {
                Id = q.Id,
                email = q.Patient?.User?.Email ?? string.Empty,
                Content = q.Content,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,

                Patient = q.Patient == null ? null! : new PatientSummaryDto
                {
                    Id = q.Patient.Id,
                    FullName = q.Patient.FirstName + " " + q.Patient.LastName,
                    ProfileImageUrl = q.Patient.User.ImageUrl
                },

                Tags = q.QuestionTags?.Select(qt => new TagDto
                {
                    Id = qt.Tag.Id,
                    Name = qt.Tag.Name
                }).ToList() ?? new List<TagDto>(),

                AnswersCount = q.Answers?.Count(a => !a.isDeleted && a.ParentAnswerId == null) ?? 0,
                InterestsCount = q.Interests?.Count ?? 0,
                SavesCount = q.SavedBy?.Count ?? 0,

                IsInterestedByCurrentUser = isInterested,
                IsSavedByCurrentUser = isSaved,
                UserStats = Stats  
            };
        }
        public async Task<List<TagDto>> GetTopTagsAsync(int topCount)
            => await _questionRepo.GetTopTagsAsync(topCount);
    }
}
