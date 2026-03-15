namespace Dactra.Repositories.Interfaces
{
    public interface IPostLikeRepository
    {
        Task<PostLike?> GetAsync(int postId, string userId);
        Task<int> GetCountByPostIdAsync(int postId);
        Task<bool> IsLikedByUserAsync(int postId, string userId);
        Task<PostLike> AddAsync(PostLike like);
        Task RemoveAsync(int postId, string userId);
    }
}
