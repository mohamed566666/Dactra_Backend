using Dactra.DTOs.QuestionDTOs;

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
        Task<UserQuestionStatsDto> GetUserStatsAsync(string userId);
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToPatientAsync(int questionId, int patientId);
        Task AssignTagsAsync(int questionId, List<int> tagIds);
    }
}
