using Dactra.Enums;

namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int? SpecializationId { get; set; }
        public Gender? Gender { get; set; }
        public bool? SortedByRating { get; set; }
        public string? SearchTerm { get; set; }
    }
}
