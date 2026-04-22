namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class WorkingHoursEntryDTO
    {
        public TimeSpan? WorkingStartTime { get; set; }
        public TimeSpan? WorkingEndTime { get; set; }
        public int? ConsultationDurationMinutes { get; set; }
        public decimal? ConsultationPrice { get; set; }
    }
}
