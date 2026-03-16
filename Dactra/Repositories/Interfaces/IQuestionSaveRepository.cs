namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionSaveRepository
    {
        Task<QuestionSave?> GetAsync(int questionId, string userId);
        Task<(List<QuestionSave> Items, int TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize);
        Task<bool> IsSavedByUserAsync(int questionId, string userId);
        Task<QuestionSave> AddAsync(QuestionSave save);
        Task RemoveAsync(int questionId, string userId);
    }
}
