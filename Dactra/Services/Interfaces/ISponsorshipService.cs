using Dactra.DTOs.Sponsorship;
using Dactra.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<ProviderOffersSummaryDTO> GetProviderOffersSummaryAsync(int providerId);
        Task<PagedSponsorshipResponseDto> GetProviderOffersPagedAsync(int providerId, int page = 1, int pageSize = 10);
        Task<PagedActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewPagedAsync(int providerId, int page = 1, int pageSize = 10);
        Task<SponsorshipResponseDTO> CancelActiveSponsorshipAsync(int sponsorshipId, int actorId, string actorRole);
        Task<PagedSponsorshipResponseDto> GetProviderOffersByFilterPagedAsync(int providerId, OfferFilterStatus filterStatus, int page = 1, int pageSize = 10);
        Task<bool> DeletePendingOfferAsync(int sponsorshipId, int providerId);
        Task<IEnumerable<DoctorMySponsorDTO>> GetMyActiveSponsorsAsync(int doctorId);
        Task<DoctorSponsorshipsResponseDTO> GetDoctorSponsorshipsWithStatsAsync(int doctorId);
        Task<bool> DeleteDoctorPendingOfferAsync(int sponsorshipId, int doctorId);
        Task<DoctorOffersSummaryDTO> GetDoctorOffersSummaryAsync(int doctorId);
        Task<PagedSponsorshipResponseDto> GetDoctorOffersByFilterPagedAsync(int doctorId, OfferFilterStatus filterStatus, int page = 1, int pageSize = 10);
    }
}