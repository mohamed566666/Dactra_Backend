namespace Dactra.DTOs.AppointmentDTOs
{
    public class CancelAppointmentRequestDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
