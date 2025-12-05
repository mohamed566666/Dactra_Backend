using Dactra.DTOs.MajorDTOs;
using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IMajorsService
    {
        Task<IEnumerable<MajorsResponseDTO>> GetAllMajorsAsync();
        Task<MajorsResponseDTO> GetMajorByIdAsync(int id);
        Task CreateMajorAsync(MajorBasicsDTO majordto);
        Task UpdateMajorAsync(int id , MajorBasicsDTO major);
        Task DeleteMajorByIdAsync(int id);
        Task UpdateMajorIconAsync(int id, string iconUrl);
    }
}
