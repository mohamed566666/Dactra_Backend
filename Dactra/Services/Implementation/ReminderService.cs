
using RTools_NTS.Util;

namespace Dactra.Services.Implementation
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IAppointmentReminderService _appointmentReminderService;

        public ReminderService(ApplicationDbContext context, INotificationService notificationService, IAppointmentReminderService appointmentReminderService)
        {
            _context = context;
            _notificationService = notificationService;
            _appointmentReminderService = appointmentReminderService;
        }


       
public async Task SendReminder(int appointmentId)
        {
            var appt = await _context.PatientAppointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appt == null || appt.IsReminderSent)
                return;

      
            var doctor = await _context.Doctors
                .Where(d => d.Id == appt.Slot.DoctorId)
                .Select(d => new { d.UserId, d.Name })
                .FirstOrDefaultAsync();

            if (doctor == null) return;

            var patientTokens = await _context.NotificationSubscriptions
                .Where(x => x.PatientId == appt.PatientId.ToString() && x.IsActive)
                .Select(x => x.FcmToken)
                .ToListAsync();

            //var doctorTokens = await _context.NotificationSubscriptions
            //    .Where(x => x.PatientId == doctor.UserId && x.IsActive)
            //    .Select(x => x.FcmToken)
            //    .ToListAsync();

            var patientUserId = await _context.Patients
                .Where(p => p.Id == appt.PatientId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();

            var utcTime = appt.Slot.SlotDateTimeUtc;

            if (patientTokens.Any())
                await _appointmentReminderService.SendBulkNotificationsAsync(
                    patientTokens, utcTime, doctor.Name, null);

            //if (doctorTokens.Any())
            //    await _appointmentReminderService.SendBulkNotificationsAsync(
            //        doctorTokens, utcTime, doctor.Name, null);

            await _notificationService.SendAsync(patientUserId,
                "1 hour left to your appointment", "Reminder", null, appt.SlotId);
            await _notificationService.SendAsync(doctor.UserId,
                "1 hour left to your appointment", "Reminder", null, appt.SlotId);

            appt.IsReminderSent = true;
            await _context.SaveChangesAsync();
            // ✅ مفيش try/catch هنا — Hangfire هيمسك الـ exception ويعيد المحاولة
        }
    }
}