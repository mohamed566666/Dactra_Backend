using Dactra.DTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IHomeRepository
    {
        Task<IEnumerable<TopRatedDoctorDTO>> GetTopRatedDoctorsAsync(int count = 10);
    }
}
