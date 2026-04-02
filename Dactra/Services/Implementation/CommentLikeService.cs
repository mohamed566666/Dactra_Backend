using Dactra.DTOs.CommentDTOs;

namespace Dactra.Services.Implementation
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly ICommentLikeRepository _likeRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IHubContext<PostHub> _hub;

        public CommentLikeService(
            ICommentLikeRepository likeRepo,
            ICommentRepository commentRepo,
            IHubContext<PostHub> hub)
        {
            _likeRepo = likeRepo;
            _commentRepo = commentRepo;
            _hub = hub;
        }

        public async Task<CommentLikeResponseDto> ToggleLikeAsync(int commentId, string userId)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId)
                ?? throw new KeyNotFoundException($"Comment {commentId} not found.");

            var isLiked = await _likeRepo.IsLikedByUserAsync(commentId, userId);
            CommentLikeResponseDto response;

            if (isLiked)
            {
                await _likeRepo.RemoveLikeAsync(commentId, userId);
                response = new CommentLikeResponseDto
                {
                    CommentId = commentId,
                    LikesCount = await _likeRepo.GetLikesCountAsync(commentId),
                    IsLikedByCurrentUser = false,
                    LikedAt = null
                };
            }
            else
            {
                var like = await _likeRepo.AddLikeAsync(commentId, userId);
                response = new CommentLikeResponseDto
                {
                    CommentId = commentId,
                    LikesCount = await _likeRepo.GetLikesCountAsync(commentId),
                    IsLikedByCurrentUser = true,
                    LikedAt = like.CreatedAt
                };
            }

            await _hub.Clients.Group(PostHub.PostGroupName(comment.PostId))
                .SendAsync("CommentLikeUpdated", new
                {
                    commentId,
                    response.LikesCount,
                    response.IsLikedByCurrentUser
                });

            return response;
        }
    }
}
