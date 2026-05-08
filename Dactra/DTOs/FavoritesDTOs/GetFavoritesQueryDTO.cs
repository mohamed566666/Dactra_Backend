namespace Dactra.DTOs.FavoritesDTOs
{
    public class GetFavoritesQueryDTO
    {
        public FavoriteType Type { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
