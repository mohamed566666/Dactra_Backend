using Dactra.DTOs.QuestionDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<UserQuestionStatsDto> GetUserQuestionStatsAsync(string userId);
        Task<Question?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<Question?> GetByIdWithDetailsAsync(int id);
        Task<(List<Question> Questions, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<(List<Question> Questions, int TotalCount)> GetByPatientIdAsync(int patientId, int page, int pageSize);
        Task<(List<Question> Questions, int TotalCount)> GetByTagAsync(int tagId, int page, int pageSize);
        Task<(List<Question> Questions, int TotalCount)> GetFilteredAsync(QuestionFilterDto filter, string userId, int page, int pageSize);
        Task<QuestionStatsDto> GetQuestionStatsAsync(int questionId);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToPatientAsync(int questionId, int patientId);
        Task AssignTagsAsync(int questionId, List<int> tagIds);
        Task<List<TagDto>> GetTopTagsAsync(int topCount);
    }
}
