using Dactra.DTOs.AppointmentDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IAppointmentStatisticsService
    {
        Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsOnlyAsync(int patientId);
        Task<PagedResultDto<PatientAppointmentListItemDto>> GetPatientAppointmentsPagedAsync(int patientId,AppointmentFilterRequestDto filter);
        Task<PatientAppointmentsStatsResponse> GetPatientFullStatsAsync(int patientId,AppointmentFilterRequestDto? filter = null);
        Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsOnlyAsync(int doctorId);
        Task<PagedResultDto<DoctorAppointmentListItemDto>> GetDoctorAppointmentsPagedAsync(int doctorId,AppointmentFilterRequestDto filter);
        Task<DoctorAppointmentsStatsResponse> GetDoctorFullStatsAsync(int doctorId,AppointmentFilterRequestDto? filter = null);
        Task<CancelAppointmentResponseDto> CancelAppointmentAsync(int appointmentId,int userId,string userRole,string reason);
        Task<DoctorWeeklyAppointmentsResponseDto> GetDoctorWeeklyAppointmentsByIdAsync(int doctorId);
        Task<DoctorWeeklyAppointmentsResponseDto> GetAuthenticatedDoctorWeeklyAppointmentsAsync(int doctorId);
    }
}
