namespace Dactra.DTOs.Sponsorship
{
    public class ActiveSponsorsOverviewDTO
    {
        public int TotalDoctors { get; set; }
        public int TotalPatientsSent { get; set; }
        public decimal AverageDiscount { get; set; }
        public List<ActiveSponsorItemDTO> Doctors { get; set; } = new();
    }
}
