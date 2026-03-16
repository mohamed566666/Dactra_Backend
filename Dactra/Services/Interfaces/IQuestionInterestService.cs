using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionInterestService
    {
        Task<QuestionInterestResponseDto> ToggleInterestAsync(int questionId, string userId);
    }
}
