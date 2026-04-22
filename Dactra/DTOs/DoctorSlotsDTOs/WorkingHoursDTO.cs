namespace Dactra.DTOs.DoctorSlotsDTOs
{
    public class WorkingHoursDTO
    {
        [Required(ErrorMessage = "Slot type is required")]
        public SlotType SlotType { get; set; } = SlotType.InPerson;

        [Required(ErrorMessage = "Working start time is required")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (24-hour)")]
        public string WorkingStartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Working end time is required")]
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$",
            ErrorMessage = "Time must be in HH:mm format (24-hour)")]
        public string WorkingEndTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Consultation duration is required")]
        [Range(10, 120, ErrorMessage = "Consultation duration must be between 10 and 120 minutes")]
        public int ConsultationDurationMinutes { get; set; } = 30;

        [Required(ErrorMessage = "Consultation price is required")]
        [Range(0, 10000, ErrorMessage = "Consultation price must be between 0 and 10000")]
        public decimal ConsultationPrice { get; set; } = 200.00m;
    }
}
