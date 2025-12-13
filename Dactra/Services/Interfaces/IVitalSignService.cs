using Dactra.DTOs.VitalSignDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IVitalSignService
    {
        Task<VitalSignResponseDTO> AddVitalSignAsync(string userId, VitalSignCreateDTO dto);
        Task<List<VitalSignResponseDTO>> GetAllForPatientAsync(string userId);
        Task<bool> DeleteVitalSignAsync(string userId, int id);
        Task<List<VitalSignType>> GetAllTypesAsync();
        Task<VitalSignType> AddTypeAsync(string name, bool isComposite, string? compositeFields);
        Task<List<VitalSignResponseDTO>> GetByPatientIdAsync(int patientId);
    }
}
