using Dactra.DTOs;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IMajorsService
    {
        Task<IEnumerable<Majors>> GetAllMajorsAsync();
        Task<Majors?> GetMajorByIdAsync(int id);
        Task<Majors> CreateMajorAsync(MajorBasicsDTO majordto);
        Task<Majors?> UpdateMajorAsync(int id , MajorBasicsDTO major);
        Task DeleteMajorAsync(int id);
    }
}
