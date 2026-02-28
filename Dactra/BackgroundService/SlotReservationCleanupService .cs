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


                    var expiredAppointments = await _context.PatientAppointments
                    .Include(a => a.Slot)
                    .Where(a => a.Status == AppointmentStatus.Pending &&
                                a.BookedAt <= DateTime.UtcNow.AddMinutes(-1))
                    .ToListAsync(stoppingToken);

                foreach (var appointment in expiredAppointments)
                {
                    appointment.Slot.IsReserved = false;
                    appointment.Slot.ReservedUntil = null;

                    _context.PatientAppointments.Remove(appointment);
                }

                if (expiredAppointments.Any())
                    await _context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // Optional: log the exception
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
