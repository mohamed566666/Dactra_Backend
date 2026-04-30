namespace Dactra.DTOs.AppointmentDTOs
{
    public class DoctorAppointmentListItemDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientImageUrl { get; set; }
        public DateTime SlotDateTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }
        public string? CancelledReason { get; set; }
    }
}
