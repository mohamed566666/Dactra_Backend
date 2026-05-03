using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dactra.Hubs
{
    public class AppointmentHub : Hub
    {

        private readonly ApplicationDbContext _context;

        public AppointmentHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task JoinMyGroup()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? Context.User?.FindFirst("sub")?.Value;

            // لو array خد الأول
            if (userId == null) return;

            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                    ?? Context.User?.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(role)) return;

            string group;

            if (role == "Doctor")
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null) return;
                group = $"Doctor_{doctor.Id}";
            }
            else
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null) return;
                group = $"Patient_{patient.Id}";
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task LeaveMyGroup()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? Context.User?.FindFirst("sub")?.Value;

            if (userId == null) return;

            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                    ?? Context.User?.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(role)) return;

            string group;

            if (role == "Doctor")
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null) return;
                group = $"Doctor_{doctor.Id}";
            }
            else
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null) return;
                group = $"Patient_{patient.Id}";
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }


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