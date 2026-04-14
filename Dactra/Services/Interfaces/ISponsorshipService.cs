using Dactra.DTOs.Sponsorship;

namespace Dactra.Services.Interfaces
{
    public interface ISponsorshipService
    {
        Task<SponsorshipResponseDTO> SendOfferAsync(int providerId, MedicalTestProviderType providerType, SendOfferDTO dto);
        Task<SponsorshipResponseDTO> AcceptOfferAsync(int sponsorshipId, int doctorId);
        Task<SponsorshipResponseDTO> RejectOfferAsync(int sponsorshipId, int doctorId);
        Task<SponsorshipResponseDTO> CounterOfferAsync(int sponsorshipId, int doctorId, CounterOfferDTO dto);
        Task<SponsorshipResponseDTO> AcceptCounterAsync(int sponsorshipId, int providerId);
        Task<SponsorshipResponseDTO> RejectCounterAsync(int sponsorshipId, int providerId);
        Task<IEnumerable<SponsorshipResponseDTO>> GetProviderOffersAsync(int providerId);
        Task<IEnumerable<SponsorshipResponseDTO>> GetActiveSponsorsForProviderAsync(int providerId);
        Task<IEnumerable<SponsorshipResponseDTO>> GetDoctorOffersAsync(int doctorId);
        Task<SponsorshipResponseDTO?> GetActiveSponsorshipAsync(int doctorId, MedicalTestProviderType type);
        Task<ProviderOffersSummaryDTO> GetProviderOffersSummaryAsync(int providerId);
        Task<IEnumerable<ProviderOfferItemDTO>> GetProviderOffersByStatusAsync(int providerId, SponsorshipStatus status);
        Task<ActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewAsync(int providerId);
        Task<SponsorshipResponseDTO> CancelActiveSponsorshipAsync(int sponsorshipId, int actorId, string actorRole);
    }
}
