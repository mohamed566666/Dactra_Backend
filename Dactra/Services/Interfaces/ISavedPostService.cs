using Dactra.DTOs.PostDTOs;

namespace Dactra.Services.Interfaces
{
    public interface ISavedPostService
    {
        Task<bool> ToggleSaveAsync(int postId, string userId);
        Task<PagedResultDto<SavedPostResponseDto>> GetSavedPostsAsync(string userId, int page, int pageSize);
    }
}
