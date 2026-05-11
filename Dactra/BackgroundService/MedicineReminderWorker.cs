namespace Dactra.BackgroundServices
{
    public class MedicineReminderWorker :BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MedicineReminderWorker> _logger;

        public MedicineReminderWorker(IServiceScopeFactory scopeFactory, ILogger<MedicineReminderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try { await CheckAndSendReminders(); }
                catch (Exception ex) { _logger.LogError(ex, "Error in MedicineReminderWorker"); }

                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }

        private async Task CheckAndSendReminders()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var fcmService = scope.ServiceProvider.GetRequiredService<IFcmService>();

            var now = TimeOnly.FromDateTime(DateTime.UtcNow);
            var today = DateTime.UtcNow.Date;

            var reminders = await context.MedicineReminders
                .Where(r => r.IsActive
                    && r.StartDate.Date <= today
                    && (r.EndDate == null || r.EndDate.Value.Date >= today))
                .ToListAsync();

            foreach (var reminder in reminders)
            {
                if (reminder.LastSentAt.HasValue)
                {
                    var diff = (DateTime.UtcNow - reminder.LastSentAt.Value).TotalMinutes;
                    if (diff < 5) continue;
                }

                var times = reminder.ScheduledTimes
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var timeStr in times)
                {
                    if (!TimeOnly.TryParse(timeStr.Trim(), out var scheduledTime)) continue;

                    var diffMinutes = Math.Abs((now - scheduledTime).TotalMinutes);
                    if (diffMinutes > 1) continue;

                    var token = await context.NotificationSubscriptions
                        .Where(s => s.PatientId == reminder.PatientId && s.IsActive)
                        .Select(s => s.FcmToken).ToListAsync();


                    if (!token.Any()) continue; 

                    await fcmService.SendBulkNotificationsAsync(
                        token,
                        "MedicineReminder",
                        $"Time to take {reminder.MedicineName} - {reminder.Dose}",
                        new Dictionary<string, string>
                        {
                        { "type", "medicine_reminder" },
                        { "reminderId", reminder.Id.ToString() },
                        { "medicineName", reminder.MedicineName }
                        });

                    reminder.LastSentAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    _logger.LogInformation("Reminder sent → patient: {P} medicine: {M}",
                        reminder.PatientId, reminder.MedicineName);
                }
            }
        }
    }
}
