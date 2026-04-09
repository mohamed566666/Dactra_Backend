using Dactra.DTOs.TagDTOs;
namespace Dactra.DTOs.PostDTOs
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public string email { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DoctorSummaryDto Doctor { get; set; } = null!;
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public int SavesCount { get; set; }
    }
}
