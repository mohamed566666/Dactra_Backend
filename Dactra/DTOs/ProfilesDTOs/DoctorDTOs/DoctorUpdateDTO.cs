namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorUpdateDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string About { get; set; }
    }
}
