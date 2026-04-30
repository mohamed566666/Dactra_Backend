namespace Dactra.Models
{
    public class VideoCallSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public PatientAppointment Appointment { get; set; } = null!;

        [Required]
        public string RoomName { get; set; } = string.Empty;

        public VideoCallStatus Status { get; set; } = VideoCallStatus.Waiting;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAtUtc { get; set; }

        public DateTime? EndedAtUtc { get; set; }

        public string? DoctorJoinedAt { get; set; }

        public string? PatientJoinedAt { get; set; }
    }
}
