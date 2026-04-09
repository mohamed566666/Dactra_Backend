using Dactra.DTOs.PostDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Services.Implementation
{
    public class PostService: IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly IPostTagRepository _postTagRepo;
        private readonly ITagRepository _tagRepo;
        private readonly IPostLikeRepository _likeRepo;
        private readonly ISavedPostRepository _savedRepo;
        private readonly IAITaggingService _aiTagging;
        private readonly IHubContext<PostHub> _hub;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository postRepo,
            IPostTagRepository postTagRepo,
            ITagRepository tagRepo,
            IPostLikeRepository likeRepo,
            ISavedPostRepository savedRepo,
            IAITaggingService aiTagging,
            IHubContext<PostHub> hub,
            ILogger<PostService> logger)
        {
            _postRepo = postRepo;
            _postTagRepo = postTagRepo;
            _tagRepo = tagRepo;
            _likeRepo = likeRepo;
            _savedRepo = savedRepo;
            _aiTagging = aiTagging;
            _hub = hub;
            _logger = logger;
        }

        public async Task<PostResponseDto> GetByIdAsync(int id, string? currentUserId = null)
        {
            var post = await _postRepo.GetByIdWithDetailsAsync(id, currentUserId)
                ?? throw new KeyNotFoundException($"Post {id} not found.");
            return await MapToResponseDtoAsync(post, currentUserId);
        }

        public async Task<PostFeedResponseDto> GetAllAsync(int page, int pageSize, string? currentUserId = null)
        {
            var (posts, total) = await _postRepo.GetAllAsync(page, pageSize, currentUserId);
            var items = new List<PostResponseDto>();
            foreach (var p in posts)
                items.Add(await MapToResponseDtoAsync(p, currentUserId));
            UserPostStatsDto stats = currentUserId != null
                ? await _postRepo.GetUserStatsAsync(currentUserId)
                : new UserPostStatsDto();

            return new PostFeedResponseDto
            {
                Posts = new PagedResultDto<PostResponseDto>
                {
                    Items = items,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize
                },
                Stats = stats
            };
        }

        public async Task<PagedResultDto<PostResponseDto>> GetByDoctorIdAsync(int doctorId, int page, int pageSize)
        {
            var (posts, total) = await _postRepo.GetByDoctorIdAsync(doctorId, page, pageSize);
            var items = new List<PostResponseDto>();
            foreach (var p in posts)
                items.Add(await MapToResponseDtoAsync(p, null));

            return new PagedResultDto<PostResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDto<PostResponseDto>> GetByTagAsync(int tagId, int page, int pageSize)
        {
            var (posts, total) = await _postRepo.GetByTagAsync(tagId, page, pageSize);
            var items = new List<PostResponseDto>();
            foreach (var p in posts)
                items.Add(await MapToResponseDtoAsync(p, null));

            return new PagedResultDto<PostResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PostResponseDto> CreateAsync(CreatePostDto dto, int doctorId)
        {
            var post = new Post
            {
                Content = dto.Content,
                DoctorId = doctorId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _postRepo.CreateAsync(post);

            // Auto-tag using AI
            await AssignTagsAsync(created.Id, dto.Content);

            // Reload with all navigation props
            var fullPost = await _postRepo.GetByIdWithDetailsAsync(created.Id)!;
            var responseDto = await MapToResponseDtoAsync(fullPost!, null);

            // Broadcast to the global feed group via SignalR
            await _hub.Clients.Group(PostHub.GlobalFeedGroup())
                .SendAsync("PostCreated", responseDto);

            return responseDto;
        }

        public async Task<PostResponseDto> UpdateAsync(int id, UpdatePostDto dto, int doctorId)
        {
            if (!await _postRepo.BelongsToDoctorAsync(id, doctorId))
                throw new UnauthorizedAccessException("You can only edit your own posts.");

            var post = await _postRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Post {id} not found.");

            post.Content = dto.Content;
            post.UpdatedAt = DateTime.UtcNow;

            await _postRepo.UpdateAsync(post);

            // Re-tag using AI with updated content
            await AssignTagsAsync(id, dto.Content);

            var fullPost = await _postRepo.GetByIdWithDetailsAsync(id)!;
            var responseDto = await MapToResponseDtoAsync(fullPost!, null);

            // Broadcast update to the post group
            await _hub.Clients.Group(PostHub.PostGroupName(id))
                .SendAsync("PostUpdated", responseDto);

            await _hub.Clients.Group(PostHub.GlobalFeedGroup())
                .SendAsync("PostUpdated", responseDto);

            return responseDto;
        }

        public async Task DeleteAsync(int id, int doctorId)
        {
            if (!await _postRepo.BelongsToDoctorAsync(id, doctorId))
                throw new UnauthorizedAccessException("You can only delete your own posts.");

            await _postRepo.SoftDeleteAsync(id);

            // Notify all clients watching this post or the feed
            await _hub.Clients.Group(PostHub.PostGroupName(id))
                .SendAsync("PostDeleted", new { postId = id });

            await _hub.Clients.Group(PostHub.GlobalFeedGroup())
                .SendAsync("PostDeleted", new { postId = id });
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task AssignTagsAsync(int postId, string content)
        {
            try
            {
                var allTags = await _tagRepo.GetAllAsync();
                var availableTagNames = allTags.Select(t => t.Name).ToList();

                var matchedNames = await _aiTagging.ExtractTagsFromContentAsync(content, availableTagNames);
                var matchedTags = await _tagRepo.GetByNamesAsync(matchedNames);

                await _postTagRepo.ReplaceTagsAsync(postId, matchedTags.Select(t => t.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto-tagging failed for post {PostId}", postId);
                // Non-fatal: post is still created/updated without tags
            }
        }

        private async Task<PostResponseDto> MapToResponseDtoAsync(Post post, string? currentUserId)
        {
            bool isLiked = currentUserId != null && await _likeRepo.IsLikedByUserAsync(post.Id, currentUserId);
            bool isSaved = currentUserId != null && await _savedRepo.IsSavedByUserAsync(post.Id, currentUserId);

            return new PostResponseDto
            {
                Id = post.Id,
                email = post.Doctor?.User?.Email,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikesCount = post.Likes?.Count ?? 0,
                CommentsCount = post.Comments?.Count ?? 0,
                SavesCount = post.SavedBy?.Count ?? 0,
                IsLikedByCurrentUser = isLiked,
                IsSavedByCurrentUser = isSaved,
                Doctor = post.Doctor == null ? null! : new DoctorSummaryDto
                {
                    Id = post.Doctor.Id,
                    FullName = post.Doctor.FirstName + ' ' + post.Doctor.LastName,
                    ProfileImageUrl = null,
                    Specialty = post.Doctor.specialization?.Name
                },
                Tags = post.PostTags?.Select(pt => new TagDto
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name
                }).ToList() ?? new List<TagDto>()
            };
        }

        public async Task<PagedResultDto<PostResponseDto>> GetMyFilteredPostsAsync(PostFilterDto filter, string userId, int page, int pageSize)
        {
            var (posts, total) = await _postRepo.GetFilteredAsync(filter, userId, page, pageSize);

            var items = new List<PostResponseDto>();
            foreach (var p in posts)
                items.Add(await MapToResponseDtoAsync(p, userId));

            return new PagedResultDto<PostResponseDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
