namespace Dactra.Models
{
    public class PostLike
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
