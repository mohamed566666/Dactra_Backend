namespace Dactra.Models
{
    public class NotificationSubscription
    {
        public int Id { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string FcmToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    }
}
