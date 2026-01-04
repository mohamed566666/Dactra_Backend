namespace Dactra.Services.Interfaces
{
    public interface IAllergyService
    {
        Task<IEnumerable<Allergy>> GetAllAsync();
        Task<Allergy?> GetByIdAsync(int id);
        Task AddAsync(string name);
        Task UpdateAsync(int id, string name);
        Task DeleteAsync(int id);
    }
}
