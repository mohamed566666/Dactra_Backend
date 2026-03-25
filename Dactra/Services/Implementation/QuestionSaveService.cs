using Dactra.DTOs.QuestionDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Implementation
{
    public class QuestionSaveService : IQuestionSaveService
    {
        private readonly IQuestionSaveRepository _saveRepo;
        private readonly IQuestionRepository _questionRepo;

        public QuestionSaveService(IQuestionSaveRepository saveRepo, IQuestionRepository questionRepo)
        {
            _saveRepo = saveRepo;
            _questionRepo = questionRepo;
        }

        public async Task<bool> ToggleSaveAsync(int questionId, string userId)
        {
            if (!await _questionRepo.ExistsAsync(questionId))
                throw new KeyNotFoundException($"Question {questionId} not found.");

            if (await _saveRepo.IsSavedByUserAsync(questionId, userId))
            {
                await _saveRepo.RemoveAsync(questionId, userId);
                return false;
            }

            await _saveRepo.AddAsync(new QuestionSave
            {
                QuestionId = questionId,
                UserId = userId,
                SavedAt = DateTime.UtcNow
            });
            return true;
        }

        public async Task<PagedResultDto<SavedQuestionResponseDto>> GetSavedQuestionsAsync(
            string userId, int page, int pageSize)
        {
            var (items, total) = await _saveRepo.GetByUserIdAsync(userId, page, pageSize);
            var dtos = items.Select(s => new SavedQuestionResponseDto
            {
                Id = s.Id,
                SavedAt = s.SavedAt,
                Question = new QuestionSummaryDto
                {
                    Id = s.Question.Id,
                    Content = s.Question.Content,
                    CreatedAt = s.Question.CreatedAt,
                    AnswersCount = s.Question.Answers?.Count(a => !a.isDeleted) ?? 0,
                    InterestsCount = s.Question.Interests?.Count ?? 0,
                    Tags = s.Question.QuestionTags?.Select(qt => new TagDto
                    {
                        Id = qt.Tag.Id,
                        Name = qt.Tag.Name
                    }).ToList() ?? new List<TagDto>()
                }
            }).ToList();

            return new PagedResultDto<SavedQuestionResponseDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
