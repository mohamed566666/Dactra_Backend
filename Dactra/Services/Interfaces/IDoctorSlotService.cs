using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorSlotService
    {
        Task SaveSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots);
        Task<DoctorSlotsDto> GetAllSlotsAsync(int doctorId);
        Task<DoctorFreeSlotsDto> GetAllFreeSlotsAsync(int doctorId);
        Task<DoctorSlotsDto> GetSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task<DoctorFreeSlotsDto> GetFreeSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task DeleteSlotsByDayAsync(int doctorId, DateTime dayUtc);
        Task DeleteSlotAsync(int doctorId, int slotId);
        Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId);
        Task<WorkingHoursResponseDTO> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours);
    }
}
