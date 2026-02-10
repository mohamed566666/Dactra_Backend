using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorSlotService
    {
        Task SaveSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots);
        Task<List<DoctorSlotResponseDto>> GetSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task<List<DoctorSlotResponseDto>> GetAllSlotsAsync(int doctorId);
        Task DeleteSlotsByDayAsync(int doctorId, DateTime dayUtc);
        Task DeleteSlotAsync(int doctorId, int slotId);
        Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId);
        Task<WorkingHoursResponseDTO> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours);
    }
}
