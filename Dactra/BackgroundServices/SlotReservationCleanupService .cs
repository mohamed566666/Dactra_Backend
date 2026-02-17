using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
namespace Dactra.BackgroundServices
{
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

                    var expiredSlots = await _context.DoctorAvailabilitySlots
                        .Where(s => s.IsReserved && s.ReservedUntil <= DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var slot in expiredSlots)
                    {
                        slot.IsReserved = false;
                        slot.ReservedUntil = null;
                    }

                    if (expiredSlots.Any())
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
}
