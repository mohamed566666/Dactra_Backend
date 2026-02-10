using Google;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dactra.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<AppointmentHub> _hub;

        public AppointmentService(
            ApplicationDbContext context,
            IHubContext<AppointmentHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<int> BookAppointmentAsync(int patientId, int slotId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var slot = await _context.DoctorAvailabilitySlots
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(x => x.Id == slotId);

                if (slot == null)
                    throw new Exception("Slot not found");

                if (slot.IsBooked)
                    throw new Exception("Slot already booked");

                if (slot.SlotDateTimeUtc <= DateTime.Now)
                    throw new Exception("Cannot book past slot");

                // Create Payment
                var payment = new Payment
                {
                    Amount = slot.Doctor.ConsultationPrice ?? throw new Exception("Consultation price is not set"),
                    Status = true,
                    Currency = "EGP",
                    Method = "Credit Card",
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                var appointment = new PatientAppointment
                {
                    PatientId = patientId,
                    PaymentId = payment.Id,
                    SlotId = slot.Id,
                    Status = AppointmentStatus.Confirmed,
                    BookedAt = DateTime.Now
                };

                _context.PatientAppointments.Add(appointment);
                await _context.SaveChangesAsync();

                slot.IsBooked = true;
                slot.AppointmentId = appointment.Id;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _hub.Clients.Group($"Doctor_{slot.DoctorId}")
                    .SendAsync("AppointmentBooked", new
                    {
                        SlotId = slot.Id,
                        AppointmentId = appointment.Id,
                        SlotDateTime = slot.SlotDateTimeUtc,
                        PatientId = patientId
                    });

                return appointment.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PatientAppointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.PatientAppointments
                .Include(a => a.Patient)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Include(a => a.Prescription)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<List<PatientAppointment>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _context.PatientAppointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.BookedAt)
                .ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int patientId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var appointment = await _context.PatientAppointments
                    .Include(a => a.Slot)
                    .FirstOrDefaultAsync(a =>
                        a.Id == appointmentId &&
                        a.PatientId == patientId);

                if (appointment == null)
                    return false;

                if (appointment.Status == AppointmentStatus.Cancelled)
                    return false;

                appointment.Status = AppointmentStatus.Cancelled;

                var slot = appointment.Slot;
                slot.IsBooked = false;
                slot.AppointmentId = null;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _hub.Clients.Group($"Doctor_{slot.DoctorId}")
                    .SendAsync("AppointmentCancelled", new
                    {
                        AppointmentId = appointment.Id,
                        SlotId = slot.Id
                    });

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
