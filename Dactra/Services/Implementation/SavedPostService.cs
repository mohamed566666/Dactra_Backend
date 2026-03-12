using Dactra.DTOs.PostDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Implementation
{
    public class SavedPostService : ISavedPostService
    {
        private readonly ISavedPostRepository _savedRepo;
        private readonly IPostRepository _postRepo;

        public SavedPostService(ISavedPostRepository savedRepo, IPostRepository postRepo)
        {
            _savedRepo = savedRepo;
            _postRepo = postRepo;
        }

        public async Task<bool> ToggleSaveAsync(int postId, string userId)
        {
            if (!await _postRepo.ExistsAsync(postId))
                throw new KeyNotFoundException($"Post {postId} not found.");

            if (await _savedRepo.IsSavedByUserAsync(postId, userId))
            {
                await _savedRepo.RemoveAsync(postId, userId);
                return false;
            }

            await _savedRepo.AddAsync(new SavedPost
            {
                PostId = postId,
                UserId = userId,
                SavedAt = DateTime.UtcNow
            });
            return true;
        }

        public async Task<PagedResultDto<SavedPostResponseDto>> GetSavedPostsAsync(string userId, int page, int pageSize)
        {
            var (items, total) = await _savedRepo.GetByUserIdAsync(userId, page, pageSize);
            var dtos = items.Select(s => new SavedPostResponseDto
            {
                Id = s.Id,
                SavedAt = s.SavedAt,
                Post = new PostSummaryDto
                {
                    Id = s.Post.Id,
                    Content = s.Post.Content,
                    CreatedAt = s.Post.CreatedAt,
                    LikesCount = s.Post.Likes?.Count ?? 0,
                    CommentsCount = s.Post.Comments?.Count ?? 0,
                    Tags = s.Post.PostTags?.Select(pt => new TagDto
                    {
                        Id = pt.Tag.Id,
                        Name = pt.Tag.Name
                    }).ToList() ?? new List<TagDto>()
                }
            }).ToList();

            return new PagedResultDto<SavedPostResponseDto>
            {
                Items = dtos,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
