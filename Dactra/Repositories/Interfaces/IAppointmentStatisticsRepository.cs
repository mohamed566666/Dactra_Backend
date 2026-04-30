using Dactra.DTOs.AppointmentDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IAppointmentStatisticsRepository
    {
        Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsAsync(int patientId);

        // فقط الإحصائيات للدكتور
        Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsAsync(int doctorId);

        // المواعيد المفهرسة للمريض مع فلتر
        Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetPatientAppointmentsPagedAsync(
            int patientId,
            AppointmentFilterRequestDto filter);

        // المواعيد المفهرسة للدكتور مع فلتر
        Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetDoctorAppointmentsPagedAsync(
            int doctorId,
            AppointmentFilterRequestDto filter);

        // جلب موعد بالتفاصيل
        Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId);

        // إلغاء موعد مع سبب
        Task<bool> CancelAppointmentWithReasonAsync(int appointmentId, string reason);
    }
}
