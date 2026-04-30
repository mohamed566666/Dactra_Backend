namespace Dactra.DTOs.AppointmentDTOs
{
    public class CancelAppointmentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RefundProcessed { get; set; }
        public decimal? RefundAmount { get; set; }
        public int AppointmentId { get; set; }
        public string? CancelledReason { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
