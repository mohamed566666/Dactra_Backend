namespace Dactra.Services.Interfaces
{
    public interface IChronicDiseaseService
    {
        Task<IEnumerable<ChronicDisease>> GetAllAsync();
        Task<ChronicDisease?> GetByIdAsync(int id);
        Task AddAsync(string name);
        Task UpdateAsync(int id, string name);
        Task DeleteAsync(int id);
    }
}
