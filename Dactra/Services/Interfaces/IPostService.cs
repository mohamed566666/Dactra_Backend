using Dactra.DTOs;
using Dactra.DTOs.PostDTOs;
namespace Dactra.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostResponseDto> GetByIdAsync(int id, string? currentUserId = null);
        Task<PostFeedResponseDto> GetAllAsync(int page, int pageSize, string? currentUserId = null);
        Task<PagedResultDto<PostResponseDto>> GetByDoctorIdAsync(int doctorId, int page, int pageSize);
        Task<PagedResultDto<PostResponseDto>> GetByTagAsync(int tagId, int page, int pageSize);
        Task<PostResponseDto> CreateAsync(CreatePostDto dto, int doctorId);
        Task<PostResponseDto> UpdateAsync(int id, UpdatePostDto dto, int doctorId);
        Task DeleteAsync(int id, int doctorId);
        Task<PagedResultDto<PostResponseDto>> GetMyFilteredPostsAsync(PostFilterDto filter, string userId, int page, int pageSize);
    }
}
