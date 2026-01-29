namespace Dactra.Services.Interfaces
{
    public interface IQuestionService
    {
        Task CreateQuestionAsync(int patientId, CreateQuestionDto dto);
        Task UpdateQuestionAsync(int questionId, int patientId, CreateQuestionDto dto);
        Task DeleteQuestionAsync(int questionId, int patientId);
        Task AnswerQuestionAsync(int questionId, int doctorId, CreateAnswerDto dto);
    }
}
