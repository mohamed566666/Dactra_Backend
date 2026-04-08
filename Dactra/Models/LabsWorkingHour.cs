namespace Dactra.Models
{
    public class LabsWorkingHour
    {
        public int Id { get; set; }

        public DayOfWeek Day { get; set; }   

        public TimeSpan From { get; set; }

        public TimeSpan To { get; set; }

        // FK
        public int ServiceProviderProfileId { get; set; }
        public ServiceProviderProfile ServiceProviderProfile { get; set; }
    }
}
