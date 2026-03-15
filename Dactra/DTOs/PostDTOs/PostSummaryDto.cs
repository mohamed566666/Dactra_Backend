using Dactra.DTOs.TagDTOs;
namespace Dactra.DTOs.PostDTOs
{
    public class PostSummaryDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public List<TagDto> Tags { get; set; } = new();
    }
}
