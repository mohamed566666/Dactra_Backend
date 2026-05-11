namespace Dactra.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription?> GetByIdAsync(int id);
        Task<Prescription?> GetByAppointmentIdAsync(int appointmentId);
        Task<bool> ExistsForAppointmentAsync(int appointmentId);
        Task<Prescription> CreateAsync(Prescription prescription);
        Task UpdateAsync(Prescription prescription);
        Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId);
        public  Task<List<Prescription>> GetByUserIdAsync(string userId);
    }
}
