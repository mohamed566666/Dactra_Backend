using Dactra.Enums;

namespace Dactra.DTOs.ProfilesDTOs
{
    public class DoctorProfileResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender gender { get; set; }
        public int SpecializationId { get; set; }
        public int age { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public string About { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
