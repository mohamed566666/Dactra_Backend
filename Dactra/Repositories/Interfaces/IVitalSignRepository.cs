namespace Dactra.Repositories.Interfaces
{
    public interface IVitalSignRepository : IGenericRepository<VitalSign>
    {
        Task<List<VitalSign>> GetByPatientIdAsync(int patientId);
        Task<List<VitalSignType>> GetAllTypesAsync();
        Task<VitalSignType?> GetTypeByIdAsync(int id);
        Task AddTypeAsync(VitalSignType type);
        Task<VitalSignType?> GetTypeByNameAsync(string name);
    }
}
