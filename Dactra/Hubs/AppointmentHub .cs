namespace Dactra.Hubs
{
    public class AppointmentHub :Hub
    {
        public async Task JoinSchedule(string scheduleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, scheduleId);
        }

        public async Task LeaveSchedule(string scheduleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, scheduleId);
        }

        public async Task JoinDoctorGroup(int doctorId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Doctor_{doctorId}");
        }

        public async Task LeaveDoctorGroup(int doctorId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Doctor_{doctorId}");
        }

        public async Task JoinPatientGroup(int patientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Patient_{patientId}");
        }

        public async Task LeavePatientGroup(int patientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Patient_{patientId}");
        }
    }
}
