namespace Dactra.Repositories.Interfaces
{
    public interface ISavedPostRepository
    {
        Task<SavedPost?> GetAsync(int postId, string userId);
        Task<(List<SavedPost> Items, int TotalCount)> GetByUserIdAsync(string userId, int page, int pageSize);
        Task<bool> IsSavedByUserAsync(int postId, string userId);
        Task<SavedPost> AddAsync(SavedPost savedPost);
        Task RemoveAsync(int postId, string userId);
    }
}
