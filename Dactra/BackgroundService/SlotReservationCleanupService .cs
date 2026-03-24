using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

public class SlotReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SlotReservationCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var expiredAppointments = await _context.PatientAppointments
                 .Where(a => a.Status == AppointmentStatus.Pending &&
                             a.BookedAt <= DateTime.UtcNow.AddMinutes(-1))
                 .Select(a => new { a.Id, a.SlotId })
                 .ToListAsync();
                var slotIds = expiredAppointments.Select(x => x.SlotId).ToList();

                await _context.DoctorAvailabilitySlots
                    .Where(s => slotIds.Contains(s.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.IsReserved, false)
                        .SetProperty(x => x.ReservedUntil, (DateTime?)null));

                await _context.PatientAppointments
                    .Where(a => expiredAppointments.Select(x => x.Id).Contains(a.Id))
                    .ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Optional: log the exception
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
