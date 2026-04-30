using Dactra.DTOs.AppointmentDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IAppointmentStatisticsService
    {
        Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsOnlyAsync(int patientId);

        // للمريض - تجيب مواعيد مفهرسة مع فلتر
        Task<PagedResultDto<PatientAppointmentListItemDto>> GetPatientAppointmentsPagedAsync(
            int patientId,
            AppointmentFilterRequestDto filter);

        // للمريض - تجيب الـ Response كامل (إحصائيات + مواعيد مفهرسة)
        Task<PatientAppointmentsStatsResponse> GetPatientFullStatsAsync(
            int patientId,
            AppointmentFilterRequestDto? filter = null);

        // للدكتور
        Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsOnlyAsync(int doctorId);
        Task<PagedResultDto<DoctorAppointmentListItemDto>> GetDoctorAppointmentsPagedAsync(
            int doctorId,
            AppointmentFilterRequestDto filter);
        Task<DoctorAppointmentsStatsResponse> GetDoctorFullStatsAsync(
            int doctorId,
            AppointmentFilterRequestDto? filter = null);

        // إلغاء موعد
        Task<CancelAppointmentResponseDto> CancelAppointmentAsync(
            int appointmentId,
            int userId,
            string userRole,
            string reason);
    }
}
