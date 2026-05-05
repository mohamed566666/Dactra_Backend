using Dactra.DTOs.AppointmentDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IAppointmentStatisticsRepository
    {
        Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsAsync(int patientId);
        Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsAsync(int doctorId);
        Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetPatientAppointmentsPagedAsync(
            int patientId,AppointmentFilterRequestDto filter);
        Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetDoctorAppointmentsPagedAsync(int doctorId,AppointmentFilterRequestDto filter);
        Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId);
        Task<bool> CancelAppointmentWithReasonAsync(int appointmentId, string reason);
        Task<List<DoctorDailyAppointmentsDto>> GetDoctorWeeklyAppointmentsAsync(int doctorId);
    }
}
