namespace Dactra.DTOs.AppointmentDTOs
{
    public class AppointmentStatisticsSummaryDto
    {
        public int Completed { get; set; }
        public int Upcoming { get; set; }
        public int Cancelled { get; set; }
        public int Unpaid { get; set; }
        public int Total { get; set; }
    }
}
