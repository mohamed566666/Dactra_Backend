using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionSaveService
    {
        Task<bool> ToggleSaveAsync(int questionId, string userId);
        Task<PagedResultDto<SavedQuestionResponseDto>> GetSavedQuestionsAsync(string userId, int page, int pageSize);
    }
}
