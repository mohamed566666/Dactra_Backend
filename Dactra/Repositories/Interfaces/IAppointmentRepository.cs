namespace Dactra.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> IsBooked(int slotId);
        Task<PatientAppointment> BookeAsync(PatientAppointment appointment);
        Task<DoctorAvailabilitySlot> GetScheduleByIdAsync(int slotId);
        Task<int> SaveChangesAsync();
        Task<PatientAppointment> UpdateAsync(PatientAppointment appointment);
    }
}
