namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class UpdateWorkingHoursDTO
    {
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (24-hour)")]
        public string? WorkingStartTime { get; set; }

        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (24-hour)")]
        public string? WorkingEndTime { get; set; }

        public int? ConsultationDurationMinutes { get; set; }
        public decimal? ConsultationPrice { get; set; }
    }
}
