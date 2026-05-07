namespace Dactra.Repositories.Interfaces
{
    public interface IPatientReferralRepository
    {
        Task<PatientReferral?> GetByIdAsync(int id);
        Task<IEnumerable<PatientReferral>> GetReferralsByProviderAsync(int providerId);
        Task<IEnumerable<PatientReferral>> GetReferralsByDoctorAsync(int doctorId);
        Task<PatientReferral> CreateAsync(PatientReferral referral);
        Task UpdateAsync(PatientReferral referral);
        Task<PagedResultDto<PatientDoctorCare>> GetActiveCarePatientsByDoctorPagedAsync(int doctorId,PaginationDto pagination,string? searchTerm = null);
        Task<PagedResultDto<PatientReferral>> GetReferralsByProviderPagedAsync(
            int providerId,
            PaginationDto pagination,
            ReferralStatus? status = null);
        Task<int> GetUniquePatientReferralCountByDoctorAsync(int doctorId);
    }
}
