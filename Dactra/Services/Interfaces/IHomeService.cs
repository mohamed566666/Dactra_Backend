using Dactra.DTOs;

namespace Dactra.Services.Interfaces
{
    public interface IHomeService
    {
        Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count = 10);
    }
}
