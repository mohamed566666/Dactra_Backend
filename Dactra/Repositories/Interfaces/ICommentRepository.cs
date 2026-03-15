namespace Dactra.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<List<Comment>> GetByPostIdAsync(int postId);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToUserAsync(int commentId, string userId);
        Task<int> GetCountByPostIdAsync(int postId);
    }
}
