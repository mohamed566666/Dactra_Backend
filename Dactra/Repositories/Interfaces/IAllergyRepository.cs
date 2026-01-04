namespace Dactra.Repositories.Interfaces
{
    public interface IAllergyRepository : IGenericRepository<Allergy>
    {
        Task<Allergy?> GetByNameAsync(string name);
    }
}
