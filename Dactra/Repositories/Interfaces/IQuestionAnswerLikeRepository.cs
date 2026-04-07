namespace Dactra.Repositories.Interfaces
{
    public interface IQuestionAnswerLikeRepository
    {
        Task<bool> IsLikedByUserAsync(int answerId, string userId);
        Task<QuestionAnswerLike?> GetLikeAsync(int answerId, string userId);
        Task<QuestionAnswerLike> AddLikeAsync(int answerId, string userId);
        Task<bool> RemoveLikeAsync(int answerId, string userId);
        Task<int> GetLikesCountAsync(int answerId);
    }
}
