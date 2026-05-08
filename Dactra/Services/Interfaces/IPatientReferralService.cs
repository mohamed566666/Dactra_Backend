using Dactra.DTOs.Sponsorship;

namespace Dactra.Services.Interfaces
{
    public interface IPatientReferralService
    {
        Task<PatientReferralResponseDTO> ReferPatientAsync(int doctorId, ReferPatientDTO dto);
        Task<PatientReferralResponseDTO> MarkAsReceivedAsync(int referralId, int providerId);
        Task<IEnumerable<PatientReferralResponseDTO>> GetReferralsByProviderAsync(int providerId);
        Task<IEnumerable<PatientReferralResponseDTO>> GetReferralsByDoctorAsync(int doctorId);
        Task<ProviderReferralsDashboardDTO> GetProviderReferralsDashboardAsync(int providerId);
        Task<PagedResultDto<DoctorCarePatientItemDTO>> GetDoctorCarePatientsAsync(int doctorId,PaginationDto pagination,string? searchTerm = null);
        Task<PagedReferralResponseDto> GetProviderReferralsPagedAsync(
            int providerId,
            int page = 1,
            int pageSize = 10,
            ReferralStatus? status = null);
    }
}
