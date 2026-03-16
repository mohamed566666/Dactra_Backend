namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionAnswerRepository
    {
        Task<QuestionAnswer?> GetByIdAsync(int id);
        Task<List<QuestionAnswer>> GetByQuestionIdAsync(int questionId);
        Task<QuestionAnswer> CreateAsync(QuestionAnswer answer);
        Task<QuestionAnswer> UpdateAsync(QuestionAnswer answer);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToDoctorAsync(int answerId, int doctorId);
        Task<int> GetActiveCountByQuestionIdAsync(int questionId);
    }
}
