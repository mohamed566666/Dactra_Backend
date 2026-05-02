using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IDoctorSlotService
    {
        Task SaveInPersonSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots);
        Task SaveOnlineSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots);
        Task<DoctorSlotsDto> GetAllInPersonSlotsAsync(int doctorId);
        Task<DoctorSlotsDto> GetAllOnlineSlotsAsync(int doctorId);
        Task<DoctorSlotsDto> GetInPersonSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task<DoctorSlotsDto> GetOnlineSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task<DoctorFreeSlotsDto> GetAllFreeInPersonSlotsAsync(int doctorId);
        Task<DoctorFreeSlotsDto> GetAllFreeOnlineSlotsAsync(int doctorId);
        Task<DoctorFreeSlotsDto> GetFreeInPersonSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task<DoctorFreeSlotsDto> GetFreeOnlineSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc);
        Task DeleteSlotsByDayAsync(int doctorId, DateTime dayUtc);
        Task DeleteSlotAsync(int doctorId, int slotId);
        Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId);
        Task<WorkingHoursResponseDTO> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours);
        Task<int> GetDoctorIdBySlotId(int slotId);




        Task<int> DeleteUnbookedPastSlotsByDoctorAsync(int doctorId);
        Task<int> DeleteAllPastSlotsByDoctorAsync(int doctorId);
        Task<int> DeleteSlotsOutsideWorkingHoursAsync(int doctorId, SlotType slotType);
        Task<int> DeleteAllSlotsOutsideWorkingHoursAsync(int doctorId); // للنوعين مع بعض
        Task<int> DeleteAllFreeSlotsByDoctorAsync(int doctorId);
    }
}
