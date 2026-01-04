namespace Dactra.Repositories.Interfaces
{
    public interface IChronicDiseaseRepository : IGenericRepository<ChronicDisease>
    {
        Task<ChronicDisease?> GetByNameAsync(string name);
    }
}
