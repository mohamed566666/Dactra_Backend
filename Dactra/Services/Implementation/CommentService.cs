using Dactra.DTOs.CommentDTOs;

namespace Dactra.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IPostRepository _postRepo;
        private readonly IHubContext<PostHub> _hub;

        public CommentService(
            ICommentRepository commentRepo,
            IPostRepository postRepo,
            IHubContext<PostHub> hub)
        {
            _commentRepo = commentRepo;
            _postRepo = postRepo;
            _hub = hub;
        }

        public async Task<List<CommentResponseDto>> GetByPostIdAsync(int postId , string? currentUserId = null)
        {
            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException($"Post {postId} not found.");

            var comments = await _commentRepo.GetByPostIdAsync(postId);
            var result = new List<CommentResponseDto>();
            foreach (var comment in comments)
            {
                result.Add(MapToDto(comment , currentUserId));
            }
            return result;
        }

        public async Task<CommentResponseDto> CreateAsync(int postId, CreateCommentDto dto, string userId)
        {
            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException($"Post {postId} not found.");

            if (dto.ParentCommentId.HasValue && !await _commentRepo.ExistsAsync(dto.ParentCommentId.Value))
                throw new KeyNotFoundException($"Parent comment {dto.ParentCommentId} not found.");

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = userId,
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _commentRepo.CreateAsync(comment);
            var responseDto = MapToDto(created , userId);

            await _hub.Clients.Group(PostHub.PostGroupName(postId))
                .SendAsync("CommentAdded", responseDto);

            return responseDto;
        }

        public async Task<CommentResponseDto> UpdateAsync(int commentId, UpdateCommentDto dto, string userId)
        {
            if (!await _commentRepo.BelongsToUserAsync(commentId, userId))
                throw new UnauthorizedAccessException("You can only edit your own comments.");

            var comment = await _commentRepo.GetByIdAsync(commentId)
                ?? throw new KeyNotFoundException($"Comment {commentId} not found.");

            comment.Content = dto.Content;
            var updated = await _commentRepo.UpdateAsync(comment);
            var responseDto = MapToDto(updated);

            await _hub.Clients.Group(PostHub.PostGroupName(comment.PostId))
                .SendAsync("CommentUpdated", responseDto);

            return responseDto;
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            if (!await _commentRepo.BelongsToUserAsync(commentId, userId))
                throw new UnauthorizedAccessException("You can only delete your own comments.");

            var comment = await _commentRepo.GetByIdAsync(commentId)
                ?? throw new KeyNotFoundException($"Comment {commentId} not found.");

            int postId = comment.PostId;
            await _commentRepo.DeleteAsync(commentId);

            await _hub.Clients.Group(PostHub.PostGroupName(postId))
                .SendAsync("CommentDeleted", new { commentId, postId });
        }

        private static CommentResponseDto MapToDto(Comment c, string? currentUserId = null) => new()
        {
            Id = c.Id,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            PostId = c.PostId,
            ParentCommentId = c.ParentCommentId,
            LikesCount = c.Likes?.Count ?? 0,
            IsLikedByCurrentUser = currentUserId != null &&
            c.Likes?.Any(l => l.UserId == currentUserId) == true,
            User = c.User == null ? null! : new UserSummaryDto
            {
                //Id = c.User.Id,
                FullName = c.User.UserName,
                ProfileImageUrl = c.User.ImageUrl
            },
            Replies = c.Replies?.Select(r => MapToDto(r, currentUserId)).ToList()
        ?? new List<CommentResponseDto>()
        };
    }
}
