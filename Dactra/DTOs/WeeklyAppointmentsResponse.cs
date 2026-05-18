namespace Dactra.DTOs
{
    public class WeeklyAppointmentsResponse
    {
        public Dictionary<string, int> DailyCounts { get; set; }

        public int OnlineCount { get; set; }

        public int OfflineCount { get; set; }

        public int TotalCount { get; set; }
    }
}
