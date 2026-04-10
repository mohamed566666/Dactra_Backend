using Dactra.DTOs.PostDTOs;
using Dactra.DTOs.TagDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<Post?> GetByIdWithDetailsAsync(int id, string? currentUserId = null);
        Task<(List<Post> Posts, int TotalCount)> GetAllAsync(int page, int pageSize, string? currentUserId = null);
        Task<(List<Post> Posts, int TotalCount)> GetByDoctorIdAsync(int doctorId, int page, int pageSize);
        Task<(List<Post> Posts, int TotalCount)> GetByTagAsync(int tagId, int page, int pageSize);
        Task<Post> CreateAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> BelongsToDoctorAsync(int postId, int doctorId);
        Task<(List<Post> Posts, int TotalCount)> GetFilteredAsync(PostFilterDto filter, string userId, int page, int pageSize);
        Task<UserPostStatsDto> GetUserStatsAsync(string userId);
        Task<List<TagDto>> GetTopTagsAsync(int topCount);
    }
}
