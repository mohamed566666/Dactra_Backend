namespace Dactra.Services.Interfaces
{
    public interface IMedicalTestsProviderService
    {
        Task ApproveProfileAsync(int id);
        Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO);
        Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId);
        Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetAllProfilesAsync(int patientId = 0);
        Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null, int patientId = 0);
        Task<MedicalTestsProviderResponseDTO> GetProfileByIdAsync(int id, int patientId = 0);
        Task<MedicalTestsProviderResponseDTO> GetProfileByUserIdAsync(string userId);
        Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetProfilesByTypeAsync(MedicalTestProviderType type, int patientId = 0);
        Task RejectProfileAsync(int id);
        Task UpdateProfileAsync(string id, MedicalTestsProviderUpdateDTO dto);
        Task<PagedResultDto<MedicalTestProviderSearchResultDTO>> SearchProvidersAsync(MedicalTestProviderSearchFilterDTO filter, int patientId = 0);
    }
}
