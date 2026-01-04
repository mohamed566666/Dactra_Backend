namespace Dactra.Models
{
    public class Allergy
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<PatientProfile> Patients { get; set; } = new List<PatientProfile>();
    }
}
