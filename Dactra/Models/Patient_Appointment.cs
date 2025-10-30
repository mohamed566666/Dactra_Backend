namespace Dactra.Models
{
    public class Patient_Appointment
    {
        public int PatientId { get; set; }
        public Patient Patient{ get; set; }

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public int PaymentId { get; set; }
        public payment Payment { get; set; }


        //public int Schedule_tableId { get; set; }

        //public Schedule_table Schedule_table { get; set; }


        public bool Status { get; set; }=false;
        public DateTime BookedAt { get; set; } = DateTime.Now;

    }
}
