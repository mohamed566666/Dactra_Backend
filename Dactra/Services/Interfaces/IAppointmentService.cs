namespace Dactra.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<int> BookAppointmentAsync(int patientId, int scheduleTableId);
    }
}
