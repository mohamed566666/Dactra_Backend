using Dactra.DTOs.QuestionDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionFeedResponseDto> GetAllAsync(int page, int pageSize, string? currentUserId = null);
        Task<QuestionResponseDto> GetByIdAsync(int id, string? currentUserId = null);
        Task<PagedResultDto<QuestionResponseDto>> GetByPatientIdAsync(int patientId, int page, int pageSize);
        Task<PagedResultDto<QuestionResponseDto>> GetByTagAsync(int tagId, int page, int pageSize);
        Task<PagedResultDto<QuestionResponseDto>> GetMyFilteredQuestionsAsync(QuestionFilterDto filter, string userId, int page, int pageSize);
        Task<PagedResultDto<QuestionResponseDto>> GetMyQuestionsAsync(int patientId, int page, int pageSize);
        Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto, int patientId);
        Task<QuestionResponseDto> UpdateAsync(int id, UpdateQuestionDto dto, int patientId);
        Task DeleteAsync(int id, int patientId);
        Task<List<TagDto>> GetTopTagsAsync(int topCount);
    }
}
