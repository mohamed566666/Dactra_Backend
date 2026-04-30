namespace Dactra.DTOs.AppointmentDTOs
{
    public class PatientAppointmentListItemDto
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;
        public string? DoctorImageUrl { get; set; }
        public DateTime SlotDateTime { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }
        public string? CancelledReason { get; set; }
    }
}
