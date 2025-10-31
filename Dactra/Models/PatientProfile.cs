using Dactra.Enums;

namespace Dactra.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public int User_ID { get; set; }
        public ApplicationUser User { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public int Age { get; set; }
        public BloodTypes BloodType { get; set; }
        public ICollection<Medicines> Current_Medications { get; set; }
        public bool IS_Smoking { get; set; }
        public string Allergies { get; set; } = string.Empty;
        public MaritalStatus MaritalStatus { get; set; }
        public string ChronicDisease { get; set; } = string.Empty;
        public List<VitalSign> VitalSign { get; set; } = new List<VitalSign>();
        public List<Patient_Appointment> Patient_Appointment { get; set; }
        public List<Questions> questions { get; set; } = new List<Questions>();
    }
}
