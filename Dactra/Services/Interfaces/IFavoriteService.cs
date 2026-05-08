using Dactra.DTOs.FavoritesDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task ToggleFavoriteAsync(int patientId, int serviceProviderId);
        Task<FavoritesResponseDTO> GetFavoritesAsync(int patientId, GetFavoritesQueryDTO query);
    }
}
