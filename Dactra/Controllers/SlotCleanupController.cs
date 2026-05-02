using Dactra.Models;
using Dactra.Services.Background;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dactra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotCleanupController : ControllerBase
    {
        private readonly ILogger<SlotCleanupController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDoctorSlotService _slotService;

        public SlotCleanupController(
            ILogger<SlotCleanupController> logger,
            IServiceProvider serviceProvider,
            IDoctorSlotService slotService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _slotService = slotService;
        }

        [HttpPost("cleanup-expired")]
        public async Task<IActionResult> CleanupExpiredSlots()
        {
            try
            {
                _logger.LogInformation("Manual cleanup of expired slots initiated.");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var results = new
                {
                    UpdatedAppointments = 0,
                    CleanedSlots = 0,
                    ClearedReservations = 0
                };

                // الخطوة 1: تحديث المواعيد المنتهية (التي مضى عليها أكثر من ساعة)
                var expiredAppointments = await context.PatientAppointments
                    .Where(a => a.Slot.CreatedAtUtc.AddHours(1) < now &&
                               a.Status != AppointmentStatus.Completed &&
                               a.Status != AppointmentStatus.Cancelled)
                    .Include(a => a.Slot)
                    .ToListAsync();

                if (expiredAppointments.Any())
                {
                    _logger.LogInformation($"Found {expiredAppointments.Count} expired appointments to update.");

                    foreach (var appointment in expiredAppointments)
                    {
                        //appointment.Status = AppointmentStatus.Missed;

                        // تحرير الـ Slot المرتبط
                        if (appointment.Slot != null)
                        {
                            appointment.Slot.IsBooked = false;
                            appointment.Slot.AppointmentId = null;
                        }
                    }

                    await context.SaveChangesAsync();
                    results = new { UpdatedAppointments = expiredAppointments.Count, CleanedSlots = 0, ClearedReservations = 0 };
                    _logger.LogInformation($"Updated {expiredAppointments.Count} expired appointments.");
                }

                // الخطوة 2: تنظيف الحجوزات المنتهية
                var expiredReservations = await context.DoctorAvailabilitySlots
                    .Where(s => s.IsReserved &&
                                s.ReservedUntil.HasValue &&
                                s.ReservedUntil.Value < now &&
                                !s.IsBooked &&
                                s.AppointmentId == null)
                    .ToListAsync();

                if (expiredReservations.Any())
                {
                    _logger.LogInformation($"Found {expiredReservations.Count} expired reservations to clear.");

                    foreach (var slot in expiredReservations)
                    {
                        slot.IsReserved = false;
                        slot.ReservedUntil = null;
                    }

                    await context.SaveChangesAsync();
                    results = new { UpdatedAppointments = expiredAppointments.Count, CleanedSlots = 0, ClearedReservations = expiredReservations.Count };
                    _logger.LogInformation($"Cleared {expiredReservations.Count} expired reservations.");
                }

                // الخطوة 3: حذف الـ slots المنتهية (بدون مواعيد)
                var expiredSlots = await context.DoctorAvailabilitySlots
                    .Where(s => s.SlotDateTimeUtc < now &&
                               !s.IsBooked &&
                               s.AppointmentId == null &&
                               !s.IsReserved)
                    .ToListAsync();

                if (expiredSlots.Any())
                {
                    _logger.LogInformation($"Found {expiredSlots.Count} expired slots to delete.");

                    context.DoctorAvailabilitySlots.RemoveRange(expiredSlots);
                    var deletedCount = await context.SaveChangesAsync();

                    results = new { UpdatedAppointments = expiredAppointments.Count, CleanedSlots = deletedCount, ClearedReservations = expiredReservations.Count };
                    _logger.LogInformation($"Successfully deleted {deletedCount} expired slots.");
                }

                var totalProcessed = expiredAppointments.Count + expiredSlots.Count + expiredReservations.Count;

                if (totalProcessed == 0)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "No expired items found to clean up",
                        Results = results,
                        Timestamp = now
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = $"Successfully cleaned up expired items: {expiredAppointments.Count} appointments updated, {expiredSlots.Count} slots deleted, {expiredReservations.Count} reservations cleared",
                    Results = results,
                    Timestamp = now
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while cleaning up expired slots");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Database error occurred",
                    Details = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error cleaning up expired slots");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to clean up expired slots",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("cleanup-appointments")]
        public async Task<IActionResult> CleanupExpiredAppointments()
        {
            try
            {
                _logger.LogInformation("Manual cleanup of expired appointments initiated.");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // العثور على المواعيد المنتهية
                var expiredAppointments = await context.PatientAppointments
                    .Where(a => a.Slot.CreatedAtUtc.AddHours(1) < now &&
                               a.Status != AppointmentStatus.Completed &&
                               a.Status != AppointmentStatus.Cancelled)
                    .Include(a => a.Slot)
                    .ToListAsync();

                if (!expiredAppointments.Any())
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "No expired appointments found",
                        Count = 0
                    });
                }

                _logger.LogInformation($"Found {expiredAppointments.Count} expired appointments.");

                // تحديث المواعيد المنتهية
                foreach (var appointment in expiredAppointments)
                {
                    //appointment.Status = AppointmentStatus.Missed;

                    // تحرير الـ Slot إذا كان موجوداً
                    if (appointment.Slot != null)
                    {
                        appointment.Slot.IsBooked = false;
                        appointment.Slot.AppointmentId = null;
                    }
                }

                var updatedCount = await context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated {updatedCount} expired appointments to Missed status.");

                return Ok(new
                {
                    Success = true,
                    Message = $"Successfully updated {updatedCount} expired appointments",
                    Count = updatedCount,
                    Timestamp = now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired appointments");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to clean up expired appointments",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("cleanup-reservations")]
        public async Task<IActionResult> CleanupExpiredReservations()
        {
            try
            {
                _logger.LogInformation("Manual cleanup of expired reservations initiated.");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // البحث عن الحجوزات المنتهية
                var expiredReservations = await context.DoctorAvailabilitySlots
                    .Where(s => s.IsReserved &&
                                s.ReservedUntil.HasValue &&
                                s.ReservedUntil.Value < now &&
                                !s.IsBooked &&
                                s.AppointmentId == null)
                    .ToListAsync();

                if (!expiredReservations.Any())
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "No expired reservations found",
                        Count = 0
                    });
                }

                _logger.LogInformation($"Found {expiredReservations.Count} expired reservations.");

                // تحديث الحجوزات المنتهية
                foreach (var slot in expiredReservations)
                {
                    slot.IsReserved = false;
                    slot.ReservedUntil = null;
                }

                var updatedCount = await context.SaveChangesAsync();

                _logger.LogInformation($"Successfully cleared {updatedCount} expired reservations.");

                return Ok(new
                {
                    Success = true,
                    Message = $"Successfully cleared {updatedCount} expired reservations",
                    Count = updatedCount,
                    Timestamp = now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired reservations");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to clean up expired reservations",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("cleanup-slots-only")]
        public async Task<IActionResult> CleanupSlotsOnly()
        {
            try
            {
                _logger.LogInformation("Manual cleanup of expired slots only initiated.");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // حذف الـ slots المنتهية فقط (بدون مواعيد)
                var expiredSlots = await context.DoctorAvailabilitySlots
                    .Where(s => s.SlotDateTimeUtc < now &&
                               !s.IsBooked &&
                               s.AppointmentId == null &&
                               !s.IsReserved)
                    .ToListAsync();

                if (!expiredSlots.Any())
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "No expired slots found to clean up",
                        Count = 0
                    });
                }

                _logger.LogInformation($"Found {expiredSlots.Count} expired slots to delete.");

                context.DoctorAvailabilitySlots.RemoveRange(expiredSlots);
                var deletedCount = await context.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted {deletedCount} expired slots.");

                return Ok(new
                {
                    Success = true,
                    Message = $"Successfully deleted {deletedCount} expired slots",
                    Count = deletedCount,
                    Timestamp = now
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while cleaning up slots");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Database error occurred",
                    Details = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired slots");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to clean up expired slots",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("doctor/{doctorId}/past-unbooked")]
        public async Task<IActionResult> DeleteUnbookedPastSlots(int doctorId)
        {
            var deletedCount = await _slotService.DeleteUnbookedPastSlotsByDoctorAsync(doctorId);
            return Ok(new
            {
                DoctorId = doctorId,
                DeletedUnbookedPastSlotsCount = deletedCount,
                Message = deletedCount == 0
                    ? "No unbooked past slots found."
                    : $"{deletedCount} unbooked past slot(s) deleted successfully."
            });
        }

        [HttpDelete("doctor/{doctorId}/all-past")]
        public async Task<IActionResult> DeleteAllPastSlots(int doctorId)
        {
            var deletedCount = await _slotService.DeleteAllPastSlotsByDoctorAsync(doctorId);
            return Ok(new
            {
                DoctorId = doctorId,
                DeletedAllPastSlotsCount = deletedCount,
                Message = deletedCount == 0
                    ? "No past slots found."
                    : $"{deletedCount} past slot(s) deleted successfully."
            });
        }

        [HttpDelete("doctor/{doctorId}/outside-hours/{slotType}")]
        public async Task<IActionResult> DeleteOutsideWorkingHours(int doctorId, string slotType)
        {
            if (!Enum.TryParse<SlotType>(slotType, true, out var type))
                return BadRequest("Invalid slot type. Use Online or InPerson");

            try
            {
                var deletedCount = await _slotService.DeleteSlotsOutsideWorkingHoursAsync(doctorId, type);
                return Ok(new
                {
                    DoctorId = doctorId,
                    SlotType = type.ToString(),
                    DeletedCount = deletedCount,
                    Message = deletedCount == 0
                        ? "No slots found outside working hours (or they were already booked)."
                        : $"{deletedCount} slot(s) deleted because they are outside working hours."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("doctor/{doctorId}/all-outside-hours")]
        public async Task<IActionResult> DeleteAllOutsideWorkingHours(int doctorId)
        {
            var totalDeleted = await _slotService.DeleteAllSlotsOutsideWorkingHoursAsync(doctorId);
            return Ok(new
            {
                DoctorId = doctorId,
                TotalDeleted = totalDeleted,
                Message = $"{totalDeleted} slot(s) deleted across both types."
            });
        }

        [HttpDelete("doctor/{doctorId}/free-slots/all")]
        public async Task<IActionResult> DeleteAllFreeSlots(int doctorId)
        {
            var deletedCount = await _slotService.DeleteAllFreeSlotsByDoctorAsync(doctorId);
            return Ok(new
            {
                DoctorId = doctorId,
                DeletedFreeSlotsCount = deletedCount,
                Message = deletedCount == 0
                    ? "No free slots found for this doctor."
                    : $"{deletedCount} free slot(s) deleted successfully."
            });
        }

        //[HttpGet("stats")]
        //public async Task<IActionResult> GetCleanupStats()
        //{
        //    try
        //    {
        //        using var scope = _serviceProvider.CreateScope();
        //        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //        var now = DateTime.UtcNow;

        //        // إحصائيات المواعيد المنتهية
        //        var expiredAppointmentsCount = await context.PatientAppointments
        //            .Where(a => a.AppointmentDateTimeUtc.AddHours(1) < now &&
        //                       a.Status != AppointmentStatus.Completed &&
        //                       a.Status != AppointmentStatus.Cancelled)
        //            .CountAsync();

        //        // إحصائيات الـ slots المنتهية (التي يمكن حذفها)
        //        var expiredSlotsCount = await context.DoctorAvailabilitySlots
        //            .Where(s => s.SlotDateTimeUtc < now &&
        //                       !s.IsBooked &&
        //                       s.AppointmentId == null &&
        //                       !s.IsReserved)
        //            .CountAsync();

        //        // إحصائيات الحجوزات المنتهية
        //        var expiredReservationsCount = await context.DoctorAvailabilitySlots
        //            .Where(s => s.IsReserved &&
        //                        s.ReservedUntil.HasValue &&
        //                        s.ReservedUntil.Value < now &&
        //                        !s.IsBooked &&
        //                        s.AppointmentId == null)
        //            .CountAsync();

        //        // إحصائيات عامة
        //        var totalSlotsCount = await context.DoctorAvailabilitySlots.CountAsync();
        //        var bookedSlotsCount = await context.DoctorAvailabilitySlots
        //            .CountAsync(s => s.IsBooked);
        //        var reservedSlotsCount = await context.DoctorAvailabilitySlots
        //            .CountAsync(s => s.IsReserved && s.ReservedUntil > now);
        //        var availableSlotsCount = await context.DoctorAvailabilitySlots
        //            .CountAsync(s => !s.IsBooked && !s.IsReserved && s.SlotDateTimeUtc > now);

        //        var totalAppointmentsCount = await context.PatientAppointments.CountAsync();
        //        var completedAppointmentsCount = await context.PatientAppointments
        //            .CountAsync(a => a.Status == AppointmentStatus.Completed);
        //        var missedAppointmentsCount = await context.PatientAppointments
        //            .CountAsync(a => a.Status == AppointmentStatus.Missed);
        //        var cancelledAppointmentsCount = await context.PatientAppointments
        //            .CountAsync(a => a.Status == AppointmentStatus.Cancelled);
        //        var scheduledAppointmentsCount = await context.PatientAppointments
        //            .CountAsync(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed);

        //        return Ok(new
        //        {
        //            Success = true,
        //            Stats = new
        //            {
        //                Expired = new
        //                {
        //                    Appointments = expiredAppointmentsCount,
        //                    Slots = expiredSlotsCount,
        //                    Reservations = expiredReservationsCount,
        //                    TotalExpiredItems = expiredAppointmentsCount + expiredSlotsCount + expiredReservationsCount
        //                },
        //                Slots = new
        //                {
        //                    Total = totalSlotsCount,
        //                    Booked = bookedSlotsCount,
        //                    Reserved = reservedSlotsCount,
        //                    Available = availableSlotsCount,
        //                    Expired = expiredSlotsCount
        //                },
        //                Appointments = new
        //                {
        //                    Total = totalAppointmentsCount,
        //                    Scheduled = scheduledAppointmentsCount,
        //                    Completed = completedAppointmentsCount,
        //                    Missed = missedAppointmentsCount,
        //                    Cancelled = cancelledAppointmentsCount,
        //                    Expired = expiredAppointmentsCount
        //                }
        //            },
        //            LastCheck = now
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting cleanup stats");
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Error = "Failed to get cleanup statistics",
        //            Details = ex.Message
        //        });
        //    }
        //}

        [HttpPost("cleanup-all")]
        public async Task<IActionResult> CleanupAll()
        {
            try
            {
                _logger.LogInformation("Full cleanup initiated (appointments, reservations, and slots).");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var results = new
                {
                    UpdatedAppointments = 0,
                    ClearedReservations = 0,
                    DeletedSlots = 0
                };

                // 1. تحديث المواعيد المنتهية
                var expiredAppointments = await context.PatientAppointments
                    .Where(a => a.Slot.SlotDateTimeUtc.AddHours(1) < now &&
                               a.Status != AppointmentStatus.Completed &&
                               a.Status != AppointmentStatus.Cancelled)
                    .Include(a => a.Slot)
                    .ToListAsync();

                if (expiredAppointments.Any())
                {
                    foreach (var appointment in expiredAppointments)
                    {
                        //appointment.Status = AppointmentStatus.Missed;
                        if (appointment.Slot != null)
                        {
                            appointment.Slot.IsBooked = false;
                            appointment.Slot.AppointmentId = null;
                        }
                    }
                    await context.SaveChangesAsync();
                    results = new { UpdatedAppointments = expiredAppointments.Count, ClearedReservations = 0, DeletedSlots = 0 };
                }

                // 2. تنظيف الحجوزات المنتهية
                var expiredReservations = await context.DoctorAvailabilitySlots
                    .Where(s => s.IsReserved &&
                                s.ReservedUntil.HasValue &&
                                s.ReservedUntil.Value < now &&
                                !s.IsBooked &&
                                s.AppointmentId == null)
                    .ToListAsync();

                if (expiredReservations.Any())
                {
                    foreach (var slot in expiredReservations)
                    {
                        slot.IsReserved = false;
                        slot.ReservedUntil = null;
                    }
                    await context.SaveChangesAsync();
                    results = new { UpdatedAppointments = expiredAppointments.Count, ClearedReservations = expiredReservations.Count, DeletedSlots = 0 };
                }

                // 3. حذف الـ slots المنتهية
                var expiredSlots = await context.DoctorAvailabilitySlots
                    .Where(s => s.SlotDateTimeUtc < now &&
                               !s.IsBooked &&
                               s.AppointmentId == null &&
                               !s.IsReserved)
                    .ToListAsync();

                if (expiredSlots.Any())
                {
                    context.DoctorAvailabilitySlots.RemoveRange(expiredSlots);
                    var deletedCount = await context.SaveChangesAsync();
                    results = new { UpdatedAppointments = expiredAppointments.Count, ClearedReservations = expiredReservations.Count, DeletedSlots = deletedCount };
                }

                var totalProcessed = expiredAppointments.Count + expiredReservations.Count + expiredSlots.Count;

                _logger.LogInformation($"Full cleanup completed: {expiredAppointments.Count} appointments, {expiredReservations.Count} reservations, {expiredSlots.Count} slots");

                return Ok(new
                {
                    Success = true,
                    Message = $"Full cleanup completed: {expiredAppointments.Count} appointments updated, {expiredReservations.Count} reservations cleared, {expiredSlots.Count} slots deleted",
                    Results = results,
                    TotalProcessed = totalProcessed,
                    Timestamp = now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during full cleanup");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to perform full cleanup",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("force-cleanup-slots")]
        public async Task<IActionResult> ForceCleanupSlots()
        {
            try
            {
                _logger.LogWarning("Force cleanup of expired slots initiated (including those with null AppointmentId).");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                // استخدام SQL مباشر لتجنب مشاكل EF Core
                var deletedCount = await context.Database.ExecuteSqlRawAsync(
                    @"DELETE FROM DoctorAvailabilitySlots 
                      WHERE SlotDateTimeUtc < {0} 
                      AND IsBooked = 0 
                      AND AppointmentId IS NULL 
                      AND IsReserved = 0",
                    now);

                _logger.LogInformation($"Force cleanup deleted {deletedCount} expired slots.");

                return Ok(new
                {
                    Success = true,
                    Message = $"Force cleanup completed. Deleted {deletedCount} expired slots.",
                    Count = deletedCount,
                    Timestamp = now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during force cleanup");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to perform force cleanup",
                    Details = ex.Message
                });
            }
        }
    }
}