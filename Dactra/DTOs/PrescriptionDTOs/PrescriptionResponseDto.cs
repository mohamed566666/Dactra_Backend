namespace Dactra.DTOs.PrescriptionDTOs
{
    public class PrescriptionResponseDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MedicineResponseDto> Medicines { get; set; } = new();
    }
}
