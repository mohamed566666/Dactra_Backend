using Dactra.DTOs.Sponsorship;

namespace Dactra.Services.Interfaces
{
    public interface ISponsorshipService
    {
        Task<SponsorshipResponseDTO> SendOfferAsync(int providerId, MedicalTestProviderType providerType, SendOfferDTO dto);
        Task<SponsorshipResponseDTO> AcceptCounterAsync(int sponsorshipId, int providerId);
        Task<SponsorshipResponseDTO> RejectCounterAsync(int sponsorshipId, int providerId);
        Task<SponsorshipResponseDTO> CancelActiveSponsorshipAsync(int sponsorshipId, int actorId, string actorRole);
        Task<SponsorshipResponseDTO> AcceptOfferAsync(int sponsorshipId, int doctorId);
        Task<SponsorshipResponseDTO> RejectOfferAsync(int sponsorshipId, int doctorId);
        Task<SponsorshipResponseDTO> CounterOfferAsync(int sponsorshipId, int doctorId, CounterOfferDTO dto);
        Task<IEnumerable<SponsorshipResponseDTO>> GetProviderOffersAsync(int providerId);
        Task<IEnumerable<SponsorshipResponseDTO>> GetActiveSponsorsForProviderAsync(int providerId);
        Task<IEnumerable<SponsorshipResponseDTO>> GetDoctorOffersAsync(int doctorId);
        Task<SponsorshipResponseDTO?> GetActiveSponsorshipAsync(int doctorId, MedicalTestProviderType type);
        Task<ProviderOffersSummaryDTO> GetProviderOffersSummaryAsync(int providerId);
        Task<IEnumerable<ProviderOfferItemDTO>> GetProviderOffersByStatusAsync(int providerId, OfferFilterStatus status);
        Task<ActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewAsync(int providerId);
        Task<PagedSponsorshipResponseDto> GetProviderOffersPagedAsync(int providerId, int page = 1, int pageSize = 10);
        Task<PagedSponsorshipResponseDto> GetDoctorOffersPagedAsync(int doctorId, int page = 1, int pageSize = 10);
        Task<PagedSponsorshipResponseDto> GetProviderOffersByStatusPagedAsync(int providerId, SponsorshipStatus? status, int page = 1, int pageSize = 10);
        Task<PagedActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewPagedAsync(int providerId, int page = 1, int pageSize = 10);
        Task<PagedSponsorshipResponseDto> GetProviderOffersByFilterPagedAsync(int providerId,OfferFilterStatus filterStatus,int page = 1,int pageSize = 10);
        Task<bool> DeletePendingOfferAsync(int sponsorshipId, int providerId);
        Task<IEnumerable<DoctorMySponsorDTO>> GetMyActiveSponsorsAsync(int doctorId);
    }
}
