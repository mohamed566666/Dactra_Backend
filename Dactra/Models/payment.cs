using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class payment
    {
        public int PayID { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Currency { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
 
        public List<Patient_Appointment> Patient_Appointment { get; set; }

    }
}
