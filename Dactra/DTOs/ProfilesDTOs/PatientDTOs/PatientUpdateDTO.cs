using Dactra.Enums;

namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNamber { get; set; }
        public int Height {  get; set; }
        public int Weight { get; set; }
        public BloodTypes BloodType { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public SmokingStatus SmokingStatus { get; set; }
        public int ? AddressId { get; set; }
    }
}
