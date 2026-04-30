namespace Dactra.DTOs.AppointmentDTOs
{
    public class PatientAppointmentsStatsResponse
    {
        public AppointmentStatisticsSummaryDto Statistics { get; set; } = new();
        public PagedResultDto<PatientAppointmentListItemDto> Appointments { get; set; } = new();
    }
}
