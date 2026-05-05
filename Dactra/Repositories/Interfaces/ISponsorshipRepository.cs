using Dactra.DTOs.Sponsorship;
using Dactra.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dactra.Repositories.Interfaces
{
    public interface ISponsorshipRepository
    {
        Task<DoctorMedicalTestSponsor?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderAsync(int providerId);
        Task<DoctorMedicalTestSponsor> CreateAsync(DoctorMedicalTestSponsor entity);
        Task UpdateAsync(DoctorMedicalTestSponsor entity);
        Task<bool> DoctorHasActiveSponsorAsync(int doctorId, MedicalTestProviderType type);
        Task<(int pending, int counter, int rejected)> GetProviderOfferCountsAsync(int providerId);
        Task<Dictionary<int, int>> GetPatientsSentCountPerDoctorAsync(int providerId);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersPagedAsync(int providerId, PaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderPagedAsync(int providerId, PaginationDto pagination);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersByFilterPagedAsync(int providerId, OfferFilterStatus filterStatus, PaginationDto pagination);
        Task<bool> DeletePendingOfferAsync(int sponsorshipId, int providerId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorshipsAsync(int doctorId);
        Task<bool> DeletePendingOfferByDoctorAsync(int sponsorshipId, int doctorId);
        Task<(int received, int counter, int rejected)> GetDoctorOfferCountsAsync(int doctorId);
        Task<PagedResultDto<DoctorMedicalTestSponsor>> GetDoctorOffersByFilterPagedAsync(int doctorId, OfferFilterStatus filterStatus, PaginationDto pagination);
    }
}