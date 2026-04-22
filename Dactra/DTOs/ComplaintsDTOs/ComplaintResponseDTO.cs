namespace Dactra.DTOs.ComplaintsDTOs
{
    public class ComplaintResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ComplaintAgainst Against { get; set; }
        public string Content { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? AdminResponse { get; set; }

        public string? UserEmail { get; set; }
    }
}
