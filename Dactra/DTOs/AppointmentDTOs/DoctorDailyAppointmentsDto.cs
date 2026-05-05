namespace Dactra.DTOs.AppointmentDTOs
{
    public class DoctorDailyAppointmentsDto
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}
