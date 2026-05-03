using Dactra.DTOs;
namespace Dactra.Repositories.Interfaces
{
    public interface ISponsorshipRepository
    {
        Task<DoctorMedicalTestSponsor?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersAsync(int providerId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersByStatusAsync(int providerId, OfferFilterStatus status);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderAsync(int providerId);
        Task<DoctorMedicalTestSponsor> CreateAsync(DoctorMedicalTestSponsor entity);
        Task UpdateAsync(DoctorMedicalTestSponsor entity);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetDoctorOffersAsync(int doctorId);
        Task<DoctorMedicalTestSponsor?> GetActiveSponsorshipAsync(int doctorId, MedicalTestProviderType type);
        Task<bool> DoctorHasActiveSponsorAsync(int doctorId, MedicalTestProviderType type);
        Task<(int pending, int counter, int rejected)> GetProviderOfferCountsAsync(int providerId);
        Task<Dictionary<int, int>> GetPatientsSentCountPerDoctorAsync(int providerId);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersPagedAsync(int providerId, PaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetDoctorOffersPagedAsync(int doctorId, PaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersByStatusPagedAsync(int providerId, StatusPaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderPagedAsync(int providerId, PaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersByFilterPagedAsync(int providerId, OfferFilterStatus filterStatus, PaginationDto pagination);
        Task<bool> DeletePendingOfferAsync(int sponsorshipId, int providerId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorshipsAsync(int doctorId);
        Task<bool> DeletePendingOfferByDoctorAsync(int sponsorshipId, int doctorId);
        Task<(int received, int counter, int rejected)> GetDoctorOfferCountsAsync(int doctorId);
    }
}
