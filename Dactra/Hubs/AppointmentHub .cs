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
    }
}
