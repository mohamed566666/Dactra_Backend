using Dactra.DTOs.CommentDTOs;

namespace Dactra.Services.Interfaces
{
    public interface ICommentLikeService
    {
        Task<CommentLikeResponseDto> ToggleLikeAsync(int commentId, string userId);
    }
}
