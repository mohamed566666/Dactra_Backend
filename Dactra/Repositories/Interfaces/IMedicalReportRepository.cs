namespace Dactra.Repositories.Interfaces
{
    public interface IMedicalReportRepository
    {
        Task<List<MedicalReport>> GetByPatientIdAsync(int patientProfileId);
        Task<MedicalReport?> GetByIdAsync(int id);
        Task AddAsync(MedicalReport report);
        void Delete(MedicalReport report);
        Task SaveChangesAsync();
    }
}
