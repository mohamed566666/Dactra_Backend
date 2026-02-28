using Google;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dactra.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<AppointmentHub> _hub;
        private readonly IPaymentService _paymentService ;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(
            ApplicationDbContext context,
            IHubContext<AppointmentHub> hub,IPaymentService paymentService, IPatientProfileRepository patientProfileRepository,IAppointmentRepository appointmentRepository)
        {
            _context = context;
            _hub = hub;
            _paymentService = paymentService;
            _patientProfileRepository = patientProfileRepository;
             _appointmentRepository = appointmentRepository;
        }

        public async Task<string> BookAppointmentAsync(int patientId, int slotId)
        {
         

            try
            {
                var now = DateTime.UtcNow;
                var expiredAppointments = await _context.PatientAppointments
                    .Include(a => a.Slot)
                    .Where(a =>
                      a.SlotId == slotId &&
                      a.Status == AppointmentStatus.Pending &&
                      a.BookedAt <= now.AddMinutes(-5))
                      .ToListAsync();

                 foreach (var appointment0 in expiredAppointments)
                 {
                    appointment0.Slot.IsReserved = false;
                    appointment0.Slot.ReservedUntil = null;
                    appointment0.Slot.IsBooked = false;
                     

                    _context.PatientAppointments.Remove(appointment0);
                 }

                   await _context.SaveChangesAsync();

                var slot = await _context.DoctorAvailabilitySlots
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(x => x.Id == slotId);

                if (slot == null)
                    throw new Exception($"Slot not found {slotId}");
                //if (slot.IsBooked || (slot.IsReserved && slot.ReservedUntil > DateTime.UtcNow))
                //{
                //    throw new Exception("Slot is currently reserved by another patient");
                //}

                if (slot.IsBooked)
                    throw new Exception("Slot already booked");

                //if (slot.SlotDateTimeUtc <= DateTime.Now)
                //    throw new Exception("Cannot book past slot");
                slot.IsReserved = true;
                slot.IsBooked=true;
                slot.ReservedUntil = DateTime.UtcNow.AddMinutes(5);


                
                // Create Payment
                var payment = new Payment
                {
                    Amount = slot.Doctor.ConsultationPrice ?? throw new Exception("Consultation price is not set"),
                    Status=paymentStatus.Pending,
                    Currency = "EGP",
                    Method = "Credit Card",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();
                var appointment = new PatientAppointment
                {
                    PatientId = patientId,
                    PaymentId = payment.Id,
                    SlotId = slot.Id,
                    Status = AppointmentStatus.Pending,
                    BookedAt = DateTime.UtcNow
                };
                  await _context.SaveChangesAsync();
                 await _appointmentRepository.BookeAsync(appointment);

                await _hub.Clients.Group($"Doctor_{slot.DoctorId}")
                    .SendAsync("AppointmentBooked", new
                    {
                        SlotId = slot.Id,
                        AppointmentId = appointment.Id,
                        SlotDateTime = slot.SlotDateTimeUtc,
                        PatientId = patientId
                    });
                var patientProfile = await _patientProfileRepository.GetByIdAsync(patientId);

                var paymentUrl = await _paymentService.GetPaymentUrl(
                    payment.Amount,
                    patientProfile.User.UserName 
                );

                return paymentUrl;
            }
            catch
            {
                
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
