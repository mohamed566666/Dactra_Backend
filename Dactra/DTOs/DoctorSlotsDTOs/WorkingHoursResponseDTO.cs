namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class WorkingHoursResponseDTO
    {
        public TimeSpan? WorkingStartTime { get; set; }
        public TimeSpan? WorkingEndTime { get; set; }
        public int? ConsultationDurationMinutes { get; set; }
        public decimal? ConsultationPrice { get; set; }
    }
}
