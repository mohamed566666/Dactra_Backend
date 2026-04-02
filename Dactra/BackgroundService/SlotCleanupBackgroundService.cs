
using Dactra.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dactra.Services.Background
{
    public class SlotCleanupBackgroundService : BackgroundService
    {
        private readonly ILogger<SlotCleanupBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _cleanupInterval;

        public SlotCleanupBackgroundService(
            ILogger<SlotCleanupBackgroundService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            _cleanupInterval = TimeSpan.FromMinutes(
                configuration.GetValue<int>("SlotCleanup:CleanupIntervalMinutes", 30));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Slot Cleanup Background Service is starting.");

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredAppointmentsAsync(stoppingToken);
                    await CleanupExpiredSlotsAsync(stoppingToken);
                    await CleanupExpiredReservationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up slots");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Slot Cleanup Background Service is stopping.");
        }

        private async Task CleanupExpiredSlotsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var hubContext = scope.ServiceProvider.GetService<IHubContext<DoctorScheduleHub>>();

                var now = DateTime.UtcNow;

                var expiredSlots = await context.DoctorAvailabilitySlots
                    .Where(s => s.SlotDateTimeUtc < now &&
                               !s.IsBooked &&
                               s.AppointmentId == null)
                    .ToListAsync(stoppingToken);

                if (!expiredSlots.Any())
                {
                    _logger.LogDebug("No expired slots found to clean up.");
                    return;
                }

                _logger.LogInformation($"Found {expiredSlots.Count} expired slots to clean up.");

                context.DoctorAvailabilitySlots.RemoveRange(expiredSlots);
                var deletedCount = await context.SaveChangesAsync(stoppingToken);

                _logger.LogInformation($"Successfully deleted {deletedCount} expired slots.");

                if (hubContext != null && deletedCount > 0)
                {
                    var doctorIds = expiredSlots.Select(s => s.DoctorId).Distinct().ToList();
                    foreach (var doctorId in doctorIds)
                    {
                        try
                        {
                            await hubContext.Clients
                                .Group($"DoctorSchedule_{doctorId}")
                                .SendAsync("SlotsUpdated", new
                                {
                                    DoctorId = doctorId,
                                    CleanedCount = expiredSlots.Count(s => s.DoctorId == doctorId),
                                    Action = "Cleanup",
                                    Timestamp = now
                                }, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Failed to send notification to doctor {doctorId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CleanupExpiredSlotsAsync");
                throw;
            }
        }

        private async Task CleanupExpiredReservationsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var hubContext = scope.ServiceProvider.GetService<IHubContext<DoctorScheduleHub>>();

                var now = DateTime.UtcNow;

                var expiredReservations = await context.DoctorAvailabilitySlots
                    .Where(s => s.IsReserved &&
                                s.ReservedUntil.HasValue &&
                                s.ReservedUntil.Value < now &&
                                !s.IsBooked &&
                                s.AppointmentId == null)
                    .ToListAsync(stoppingToken);

                if (!expiredReservations.Any())
                {
                    _logger.LogDebug("No expired reservations found.");
                    return;
                }

                _logger.LogInformation($"Found {expiredReservations.Count} expired reservations.");

                foreach (var slot in expiredReservations)
                {
                    slot.IsReserved = false;
                    slot.ReservedUntil = null;
                }

                var updatedCount = await context.SaveChangesAsync(stoppingToken);

                _logger.LogInformation($"Successfully cleared {updatedCount} expired reservations.");

                if (hubContext != null && updatedCount > 0)
                {
                    var doctorIds = expiredReservations.Select(s => s.DoctorId).Distinct().ToList();
                    foreach (var doctorId in doctorIds)
                    {
                        try
                        {
                            await hubContext.Clients
                                .Group($"DoctorSchedule_{doctorId}")
                                .SendAsync("ReservationsExpired", new
                                {
                                    DoctorId = doctorId,
                                    Message = "Some reservations have expired",
                                    Timestamp = now
                                }, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Failed to send reservation expiration notification to doctor {doctorId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CleanupExpiredReservationsAsync");
                throw;
            }
        }

        private async Task CleanupExpiredAppointmentsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var expiredAppointments = await context.PatientAppointments
                    .Where(a => a.Slot.SlotDateTimeUtc.AddHours(1) < now &&
                               a.Status != AppointmentStatus.Completed &&
                               a.Status != AppointmentStatus.Cancelled)
                    .Include(a => a.Slot)
                    .ToListAsync(stoppingToken);

                if (!expiredAppointments.Any())
                {
                    _logger.LogDebug("No expired appointments found.");
                    return;
                }

                _logger.LogInformation($"Found {expiredAppointments.Count} expired appointments to mark as missed.");

                foreach (var appointment in expiredAppointments)
                {
                    //appointment.Status = AppointmentStatus.Missed;

                    if (appointment.Slot != null)
                    {
                        appointment.Slot.IsBooked = false;
                        appointment.Slot.AppointmentId = null;
                    }
                }

                var updatedCount = await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Successfully marked {updatedCount} appointments as missed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CleanupExpiredAppointmentsAsync");
                throw;
            }
        }
    }
}