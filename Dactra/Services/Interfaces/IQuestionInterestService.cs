using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionInterestService
    {
        Task<QuestionInterestResponseDto> ToggleInterestAsync(int questionId, string userId);
        Task<List<string>> GetInterestedUsersIdAsync(int questionId);
    }
}
