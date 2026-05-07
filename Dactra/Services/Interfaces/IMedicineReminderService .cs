namespace Dactra.Services.Interfaces
{
    public interface IMedicineReminderService
    {
        
       Task <int> CreateFromPrescriptionAsync(int appointmentId);
           
        
    }
}
