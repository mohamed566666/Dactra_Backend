namespace Dactra.DTOs.Sponsorship
{
    public class DoctorCarePatientsResponseDTO
    {
        public int TotalPatients { get; set; }
        public List<DoctorCarePatientItemDTO> Patients { get; set; } = new();
    }
}
