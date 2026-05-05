namespace Dactra.DTOs.PrescriptionDTOs
{
    public class MedicineResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Dose { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DurationDisplay => $"{Duration} days";
        public TimesPerDay TimesPerDay { get; set; }
        public string TimesPerDayDisplay { get; set; } = string.Empty;
        public WhenToTake WhenToTake { get; set; }
        public string WhenToTakeDisplay { get; set; } = string.Empty;
        public TimeSpan FirstDoseTime { get; set; }
        public string FirstDoseTimeDisplay { get; set; } = string.Empty;
        public List<DoseTimeResponseDto> DoseTimes { get; set; } = new();
    }
}
