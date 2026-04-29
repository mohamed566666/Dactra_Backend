namespace Dactra.Hubs
{
    [Authorize]
    public class VideoCallHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public VideoCallHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task JoinAppointmentGroup(int appointmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Appointment_{appointmentId}");
        }

        public async Task LeaveAppointmentGroup(int appointmentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Appointment_{appointmentId}");
        }
        public async Task NotifyCallStarted(int appointmentId)
        {
            await Clients.OthersInGroup($"Appointment_{appointmentId}")
                .SendAsync("IncomingCall", new { AppointmentId = appointmentId });
        }

    }
}
