using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionAnswerService
    {
        Task<PagedResultDto<AnswerResponseDto>> GetTopLevelAnswersByQuestionIdAsync(
             int questionId, int page, int pageSize, string? currentUserId = null);
        Task<PagedResultDto<AnswerResponseDto>> GetRepliesByParentAnswerIdAsync(
            int parentAnswerId, int page, int pageSize, string? currentUserId = null);
        Task<AnswerResponseDto> CreateAsync(int questionId, CreateAnswerDto dto, string userId, bool isDoctor);
        Task<AnswerResponseDto> UpdateAsync(int answerId, UpdateAnswerDto dto, string userId, bool isDoctor);
        Task DeleteAsync(int answerId, string userId, bool isDoctor);
    }
}
