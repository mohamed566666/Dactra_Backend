namespace Dactra.DTOs.Sponsorship
{
    public class PagedActiveSponsorsOverviewDTO
    {
        public List<ActiveSponsorItemDTO> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatientsSent { get; set; }
        public double AverageDiscount { get; set; }
    }
}
