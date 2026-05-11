namespace Dactra.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task ToggleFavoriteAsync(int patientId, int serviceProviderId);
        Task<bool> IsFavoriteAsync(int patientId, int serviceProviderId);
        Task<List<int>> GetFavoriteServiceProviderIdsAsync(int patientId, List<int> serviceProviderIds);
        Task<(int TotalCount, List<DoctorProfile> Items)> GetFavoriteDoctorsPagedAsync(int patientId, int skip, int take);
        Task<(int TotalCount, List<MedicalTestProviderProfile> Items)> GetFavoriteProvidersPagedAsync(int patientId, int skip, int take);
    }
}
