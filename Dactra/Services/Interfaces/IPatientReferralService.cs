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
        Task<DoctorCarePatientsResponseDTO> GetDoctorCarePatientsAsync(int doctorId);
        Task<PagedReferralResponseDto> GetProviderReferralsPagedAsync(
            int providerId,
            int page = 1,
            int pageSize = 10,
            ReferralStatus? status = null);
    }
}
