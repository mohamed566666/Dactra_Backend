namespace Dactra.DTOs.AppointmentDTOs
{
    public class AppointmentFilterRequestDto : PaginationDto
    {
        public AppointmentStatus? Status { get; set; }
        public SlotType? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool UpcomingOnly { get; set; } = false;
    }
}
