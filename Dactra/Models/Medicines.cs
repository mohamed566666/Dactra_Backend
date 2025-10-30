namespace Dactra.Models
{
    public class Medicines
    {
        public int med_id { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public List<Prescription> prescriptions { get; set; } = new List<Prescription>();
    }
}
