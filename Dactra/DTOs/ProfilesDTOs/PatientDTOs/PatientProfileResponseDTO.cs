namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientProfileResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string PhoneNumber { get; set;}
        public string Email { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public BloodTypes BloodType { get; set; }
        public SmokingStatus SmokingStatus { get; set; }
        public int? AddressId { get; set; }
        public string? address { get; set; }
        public List<string> Allergies { get; set; } = new();
        public List<string> ChronicDiseases { get; set; } = new();
        public string RoleName { get; set; }
    }
}
