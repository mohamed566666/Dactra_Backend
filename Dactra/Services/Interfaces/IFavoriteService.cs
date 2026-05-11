using Dactra.DTOs.FavoritesDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task ToggleFavoriteAsync(int patientId, int serviceProviderId);
        Task<bool> IsFavoriteAsync(int patientId, int serviceProviderId);
        Task<List<int>> GetFavoriteServiceProviderIdsAsync(int patientId, List<int> serviceProviderIds);
        Task<FavoritesResponseDTO> GetFavoritesAsync(int patientId, GetFavoritesQueryDTO query);
    }
}
