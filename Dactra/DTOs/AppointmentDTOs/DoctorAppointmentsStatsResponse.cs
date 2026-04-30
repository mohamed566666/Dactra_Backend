namespace Dactra.DTOs.AppointmentDTOs
{
    public class DoctorAppointmentsStatsResponse
    {
        public AppointmentStatisticsSummaryDto Statistics { get; set; } = new();
        public PagedResultDto<DoctorAppointmentListItemDto> Appointments { get; set; } = new();
    }
}
