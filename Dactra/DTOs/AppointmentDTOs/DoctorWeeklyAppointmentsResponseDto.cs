namespace Dactra.DTOs.AppointmentDTOs
{
    public class DoctorWeeklyAppointmentsResponseDto
    {
        public int DoctorId { get; set; }
        public List<DoctorDailyAppointmentsDto> DailyCounts { get; set; } = new();
        public int TotalAppointments { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
