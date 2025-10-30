using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class Prescription
    {
        public int QID { get; set; }
        public string Diagnosis { get; set; } = string.Empty;

        public List<Medicines> Medicines { get; set; } = new List<Medicines>();
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public List<Patient_Appointment> Patient_Appointment { get; set; }
    }
}
