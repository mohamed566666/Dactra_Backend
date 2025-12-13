namespace Dactra.Repositories.Interfaces
{
    public interface IRatingRepository : IGenericRepository<Rating>
    {
        Task<Rating?> GetByPatientAndProviderAsync(int patientId, int providerId);
        Task<List<Rating>> GetByProviderIdAsync(int providerId);
        Task<List<Rating>> GetByPatientIdAsync(int patientId);
    }
}
