using Dactra.Enums;

namespace Dactra.Models
{
    public class Schedule_table
    {
        public int Schedule_tableId { get; set; }

        public List<Patient_Appointment> Patient_Appointment { get; set; }

        public int DoctorId { get; set; }
        public DoctorProfile Doctor{ get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Schedule_Mode Type { get; set; }


        public List<bool> BookedStatuses
        {
            get
            {
                return Patient_Appointment.Select(d => d.Status).ToList();
            }
        }

    }
}
