namespace Dactra.Repositories.Interfaces
{
    public interface IPatientReferralRepository
    {
        Task<PatientReferral?> GetByIdAsync(int id);
        Task<IEnumerable<PatientReferral>> GetReferralsByProviderAsync(int providerId);
        Task<IEnumerable<PatientReferral>> GetReferralsByDoctorAsync(int doctorId);
        Task<PatientReferral> CreateAsync(PatientReferral referral);
        Task UpdateAsync(PatientReferral referral);
        Task<IEnumerable<PatientDoctorCare>> GetActiveCarePatientsByDoctorAsync(int doctorId);
        Task<PagedResultDto<PatientReferral>> GetReferralsByProviderPagedAsync(
            int providerId,
            PaginationDto pagination,
            ReferralStatus? status = null);
    }
}
