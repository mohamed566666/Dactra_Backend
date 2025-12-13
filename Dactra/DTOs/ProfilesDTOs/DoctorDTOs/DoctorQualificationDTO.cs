namespace Dactra.DTOs.ProfilesDTOs.DoctorDTOs
{
    public class DoctorQualificationDTO
    {
        [Required]
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
