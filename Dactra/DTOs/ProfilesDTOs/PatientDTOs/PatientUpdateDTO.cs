namespace Dactra.DTOs.ProfilesDTOs.PatientDTOs
{
    public class PatientUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNamber { get; set; }
        [Range(40, 250)]
        public int Height {  get; set; }
        [Range(20, 500)]
        public int Weight { get; set; }
        [Range(0, 8)]
        public BloodTypes BloodType { get; set; }
        [Range(0, 4)]
        public MaritalStatus MaritalStatus { get; set; }
        [Range(0, 3)]
        public SmokingStatus SmokingStatus { get; set; }
        public int ? AddressId { get; set; }
    }
}
