namespace Dactra.Models
{
    public class Notifications
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!; 

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;

        public string? Type { get; set; } 

        public int? RelatedEntityId { get; set; } 

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
