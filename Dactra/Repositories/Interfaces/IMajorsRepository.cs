using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface IMajorsRepository
    {
        Task<Majors> GetByIdAsync(int id);
        Task<IEnumerable<Majors>> GetAllAsync();
        Task AddAsync(Majors major);
        Task UpdateAsync(Majors major);
        Task DeleteAsync(Majors major);
        Task UpdateIcon(int id, string iconUrl);
    }
}
