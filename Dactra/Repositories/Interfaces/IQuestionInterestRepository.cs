namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionInterestRepository
    {
        Task<QuestionInterest?> GetAsync(int questionId, string userId);
        Task<int> GetCountByQuestionIdAsync(int questionId);
        Task<bool> IsInterestedByUserAsync(int questionId, string userId);
        Task<List<string>> GetInterestedUserIdsByQuestionIdAsync(int questionId);
        Task<QuestionInterest> AddAsync(QuestionInterest interest);
        Task RemoveAsync(int questionId, string userId);
    }
}
