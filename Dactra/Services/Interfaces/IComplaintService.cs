using Dactra.DTOs.ComplaintsDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IComplaintService
    {
        Task CreateAsync(string userId, CreateComplaintDTO dto);
        Task<IEnumerable<ComplaintResponseDTO>> GetMyComplaintsAsync(string userId);
        Task<IEnumerable<ComplaintResponseDTO>> GetAllComplaintsAsync();
        Task<ComplaintResponseDTO> GetDetailsAsync(int id);
        Task UpdateStatusAsync(int id, string adminId, UpdateComplaintStatusDTO dto);
    }
}
