using Dactra.DTOs.CommentDTOs;

namespace Dactra.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentResponseDto>> GetByPostIdAsync(int postId , string? currentUserId = null);
        Task<CommentResponseDto> CreateAsync(int postId, CreateCommentDto dto, string userId);
        Task<CommentResponseDto> UpdateAsync(int commentId, UpdateCommentDto dto, string userId);
        Task DeleteAsync(int commentId, string userId);
    }
}
