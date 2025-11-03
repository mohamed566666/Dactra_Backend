using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }
        public string Diagnosis { get; set; } = string.Empty;

        public List<Medicines> Medicines { get; set; } = new List<Medicines>();

        public List<PatientAppointment> PatientAppointment { get; set; } = new List<PatientAppointment>();
    }
}
