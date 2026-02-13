namespace Dactra.Hubs
{
    public class DoctorScheduleHub : Hub
    {
        public async Task JoinDoctorSchedule(int doctorId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"DoctorSchedule_{doctorId}");
        }

        public async Task LeaveDoctorSchedule(int doctorId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"DoctorSchedule_{doctorId}");
        }
    }
}
