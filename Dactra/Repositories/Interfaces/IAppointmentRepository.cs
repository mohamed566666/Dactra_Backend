namespace Dactra.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<bool> IsBooked(int scheduletableId);
        Task<PatientAppointment> BookeAsync(PatientAppointment appointment);
    }
}
