namespace Dactra.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<String> BookAppointmentAsync(int patientId, int slotId);
        Task<PatientAppointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<List<PatientAppointment>> GetPatientAppointmentsAsync(int patientId);
        Task<bool> CancelAppointmentAsync(int appointmentId, int patientId);
        Task<List<DoctorAppointmentDto>> GetDoctorAppointmentsAsync(int doctorId);
    }
}
