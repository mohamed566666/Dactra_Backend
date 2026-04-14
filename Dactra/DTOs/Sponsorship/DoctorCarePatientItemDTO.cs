namespace Dactra.DTOs.Sponsorship
{
    public class DoctorCarePatientItemDTO
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
