using Dactra.Enums;
using System.Globalization;

namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorProfileResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Gender gender { get; set; }
        public string SpecializationName { get; set; }
        public int age { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public string About { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
