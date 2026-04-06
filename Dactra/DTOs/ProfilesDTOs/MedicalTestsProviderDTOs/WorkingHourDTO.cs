namespace Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs
{
    public class WorkingHourDTO
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}

