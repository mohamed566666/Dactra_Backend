using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionAnswerLikeService
    {
        Task<AnswerLikeResponseDto> ToggleLikeAsync(int answerId, string userId);
    }
}
