namespace Dactra.Models
{
    public class Patient
    {
        public int  Id { get; set; }
        public int  Height{ get; set; }
        public int Weight { get; set; }
        public string BloodType { get; set; }=string.Empty;
        public string Current_Medications { get; set; }=string.Empty;
        public bool Smoking { get; set; }
        public string Allergies { get; set; }= string.Empty ;
        public string MaritalStatus { get; set; } = string.Empty;
        public string ChronicDisease { get; set; } = string.Empty;
        public List<VitalSign> VitalSign { get; set; } = new List<VitalSign>();
  

        public List<Patient_Appointment> Patient_Appointment { get; set; }

        public List<Questions>questions { get; set; }=new List<Questions>();
    }
}
