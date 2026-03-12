namespace Dactra.DTOs.PostDTOs
{
    public class SavedPostResponseDto
    {
        public int Id { get; set; }
        public PostSummaryDto Post { get; set; } = null!;
        public DateTime SavedAt { get; set; }
    }
}
