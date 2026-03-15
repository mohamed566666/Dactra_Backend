using Dactra.DTOs.PostLikeDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IPostLikeService
    {
        Task<PostLikeResponseDto> ToggleLikeAsync(int postId, string userId);
    }
}
