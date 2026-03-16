using Dactra.DTOs.QuestionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IQuestionAnswerService
    {
        Task<List<AnswerResponseDto>> GetByQuestionIdAsync(int questionId);
        Task<AnswerResponseDto> CreateAsync(int questionId, CreateAnswerDto dto, int doctorId);
        Task<AnswerResponseDto> UpdateAsync(int answerId, UpdateAnswerDto dto, int doctorId);
        Task DeleteAsync(int answerId, int doctorId);
    }
}
