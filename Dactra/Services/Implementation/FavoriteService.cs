using Dactra.DTOs.FavoritesDTOs;

namespace Dactra.Services.Implementation
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task ToggleFavoriteAsync(int patientId, int serviceProviderId)
        {
            await _favoriteRepository.ToggleFavoriteAsync(patientId, serviceProviderId);
        }

        public async Task<FavoritesResponseDTO> GetFavoritesAsync(int patientId, GetFavoritesQueryDTO query)
        {
            int skip = (query.Page - 1) * query.PageSize;

            if (query.Type == FavoriteType.Doctor)
            {
                var (totalCount, items) = await _favoriteRepository.GetFavoriteDoctorsPagedAsync(patientId, skip, query.PageSize);

                return new FavoritesResponseDTO
                {
                    Items = items.Select(d => new DoctorsFilterResponseDTO
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Specialization = d.specialization?.Name ?? string.Empty,
                        AverageRating = d.Avg_Rating,
                        profileImageUrl = d.User?.ImageUrl,
                        OfflinePrice = d.ConsultationPrice,
                        OnlinePrice = d.OnlineConsultationPrice
                    }).ToList(),
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                };
            }
            else if (query.Type == FavoriteType.MedicalProvider)
            {
                var (totalCount, items) = await _favoriteRepository.GetFavoriteProvidersPagedAsync(patientId, skip, query.PageSize);

                return new FavoritesResponseDTO
                {
                    Items = items.Select(p => new MedicalTestProviderSearchResultDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Type = (int)p.Type,
                        Address = p.Address,
                        ImageUrl = p.User?.ImageUrl,
                        Rating = p.Avg_Rating
                    }).ToList(),
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                };
            }

            throw new ArgumentException("Invalid favorite type");
        }
    }
}
