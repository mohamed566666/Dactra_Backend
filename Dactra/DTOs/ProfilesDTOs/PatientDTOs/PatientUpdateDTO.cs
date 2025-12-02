using Dactra.Enums;

namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNamber { get; set; }
        public Gender gender { get; set; }
        public int Height {  get; set; }
        public int Weight { get; set; }
        public BloodTypes BloodType { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public SmokingStatus SmokingStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Allergies { get; set; } = string.Empty;
        public string ChronicDisease { get; set; } = string.Empty;
    }
}
