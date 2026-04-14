namespace Dactra.Repositories.Interfaces
{
    public interface ISponsorshipRepository
    {
        Task<DoctorMedicalTestSponsor?> GetByIdAsync(int id);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersAsync(int providerId);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersByStatusAsync(int providerId, SponsorshipStatus status);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderAsync(int providerId);
        Task<DoctorMedicalTestSponsor> CreateAsync(DoctorMedicalTestSponsor entity);
        Task UpdateAsync(DoctorMedicalTestSponsor entity);
        Task<IEnumerable<DoctorMedicalTestSponsor>> GetDoctorOffersAsync(int doctorId);
        Task<DoctorMedicalTestSponsor?> GetActiveSponsorshipAsync(int doctorId, MedicalTestProviderType type);
        Task<bool> DoctorHasActiveSponsorAsync(int doctorId, MedicalTestProviderType type);
        Task<(int pending, int counter, int rejected)> GetProviderOfferCountsAsync(int providerId);
        Task<Dictionary<int, int>> GetPatientsSentCountPerDoctorAsync(int providerId);
    }
}
