namespace Dactra.Repositories.Interfaces
{
    public interface ICommentLikeRepository
    {
        Task<bool> IsLikedByUserAsync(int commentId, string userId);
        Task<CommentLike?> GetLikeAsync(int commentId, string userId);
        Task<CommentLike> AddLikeAsync(int commentId, string userId);
        Task<bool> RemoveLikeAsync(int commentId, string userId);
        Task<int> GetLikesCountAsync(int commentId);
    }
}
