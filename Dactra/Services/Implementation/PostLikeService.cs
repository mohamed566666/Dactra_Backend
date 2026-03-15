using Dactra.DTOs.PostLikeDTOs;

namespace Dactra.Services.Implementation
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IPostLikeRepository _likeRepo;
        private readonly IPostRepository _postRepo;
        private readonly IHubContext<PostHub> _hub;

        public PostLikeService(
            IPostLikeRepository likeRepo,
            IPostRepository postRepo,
            IHubContext<PostHub> hub)
        {
            _likeRepo = likeRepo;
            _postRepo = postRepo;
            _hub = hub;
        }

        public async Task<PostLikeResponseDto> ToggleLikeAsync(int postId, string userId)
        {
            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException($"Post {postId} not found.");

            bool isLiked;
            if (await _likeRepo.IsLikedByUserAsync(postId, userId))
            {
                await _likeRepo.RemoveAsync(postId, userId);
                isLiked = false;
            }
            else
            {
                await _likeRepo.AddAsync(new PostLike
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
                isLiked = true;
            }

            var totalLikes = await _likeRepo.GetCountByPostIdAsync(postId);
            var response = new PostLikeResponseDto
            {
                PostId = postId,
                TotalLikes = totalLikes,
                IsLiked = isLiked
            };

            await _hub.Clients.Group(PostHub.PostGroupName(postId))
                .SendAsync("LikeUpdated", response);

            return response;
        }
    }
}
