namespace Dactra.Models
{
    public class Medicines
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Indication { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Prescription> prescriptions { get; set; } = new List<Prescription>();
        public List<PrescriptionWithMedicin> PrescriptionWithMedicins { get; set; } = new List<PrescriptionWithMedicin>();
    }
}
