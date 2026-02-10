namespace Dactra.Services.Interfaces
{
    public interface ISlotGeneratorService
    {
        Task GenerateSlotsForDoctorAsync(int doctorId, DateTime date);

        Task GenerateSlotsForAllDoctorsAsync(DateTime date);
    }
}
