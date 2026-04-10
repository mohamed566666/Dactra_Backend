namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionAnswerRepository
    {
        Task<(List<QuestionAnswer> Answers, int TotalCount)> GetTopLevelAnswersByQuestionIdAsync(
            int questionId, int page, int pageSize);
        Task<(List<QuestionAnswer> Replies, int TotalCount)> GetRepliesByParentAnswerIdAsync(
            int parentAnswerId, int page, int pageSize);
        Task<int> GetRepliesCountAsync(int parentAnswerId);
        Task<QuestionAnswer?> GetByIdAsync(int id);
        Task<List<QuestionAnswer>> GetByQuestionIdAsync(int questionId);
        Task<QuestionAnswer> CreateAsync(QuestionAnswer answer);
        Task<QuestionAnswer> UpdateAsync(QuestionAnswer answer);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToUserAsync(int answerId, string userId);
        Task<Question?> GetQuestionByAnswerIdAsync(int answerId);
    }
}
