using Google;
using Hangfire;
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
        private readonly IReminderService _reminderService;

        public AppointmentService(
            ApplicationDbContext context,
            IHubContext<AppointmentHub> hub,IPaymentService paymentService, IPatientProfileRepository patientProfileRepository,IAppointmentRepository appointmentRepository, IReminderService reminderService)
        {
            _context = context;
            _hub = hub;
            _paymentService = paymentService;
            _patientProfileRepository = patientProfileRepository;
            _appointmentRepository = appointmentRepository;
            _reminderService = reminderService;
        }

        public async Task<string> BookAppointmentAsync(int patientId, int slotId)
        {
         

            try
            {

                using var tx = await _context.Database.BeginTransactionAsync();

                var slot = await _context.DoctorAvailabilitySlots
                          .Where(s => s.Id == slotId && !s.IsBooked)
                          .Include(s => s.Doctor)
                          .FirstOrDefaultAsync();

                if (slot == null || slot.IsBooked)
                    throw new Exception("Slot already booked");

                slot.IsBooked = true;
                slot.IsReserved = true;
                slot.ReservedUntil = DateTime.UtcNow.AddMinutes(5);

                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                //if (slot.IsBooked || (slot.IsReserved && slot.ReservedUntil > DateTime.UtcNow))
                //{
                //    throw new Exception("Slot is currently reserved by another patient");
                //}



                //if (slot.SlotDateTimeUtc <= DateTime.Now)
                //    throw new Exception("Cannot book past slot");




                // Create Payment
                var amount = slot.SlotType == SlotType.InPerson
                    ? slot.Doctor.ConsultationPrice
                    : slot.Doctor.OnlineConsultationPrice
                    ?? throw new Exception("Price is not set");
                var payment = new Payment
                {
                    Amount = amount ?? throw new Exception("Consultation price is not set"),
                    Status=paymentStatus.Pending,
                    Currency = "EGP",
                    Method = "Credit Card",
                    CreatedAt = DateTime.UtcNow,
                    isRefunded=false
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

                 await _appointmentRepository.BookeAsync(appointment);
                await CreateOrRenewCareAsync(patientId, slot.DoctorId);

                var utcTime = DateTime.SpecifyKind(slot.SlotDateTimeUtc, DateTimeKind.Local)
                      .ToUniversalTime();

                if (utcTime <= DateTime.UtcNow)
                    throw new Exception("Slot time is in the past");

                var reminderTime = utcTime.AddHours(-1);
                var delay = reminderTime - DateTime.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    var jobId = BackgroundJob.Schedule<ReminderService>(
                        x => x.SendReminder(appointment.Id),
                        delay
                    );

                    appointment.ReminderJobId = jobId;
                }
                else
                {
                    await _reminderService.SendReminder(appointment.Id);
                }

                await _context.SaveChangesAsync();

                
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
                    payment,
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
                        a.SlotId == appointmentId &&
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
                await _paymentService.RefundAppointmentAsync(slot.Id);
                if (!string.IsNullOrEmpty(appointment.ReminderJobId))
                {
                    BackgroundJob.Delete(appointment.ReminderJobId);
                }

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

        private async Task CreateOrRenewCareAsync(int patientId, int doctorId)
        {
            var existing = await _context.PatientDoctorCares
                .FirstOrDefaultAsync(x => x.PatientId == patientId
                                        && x.DoctorId == doctorId);

            if (existing is null)
            {
                _context.PatientDoctorCares.Add(new PatientDoctorCare
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    CreatedAtUtc = DateTime.UtcNow,
                    ExpiresAtUtc = DateTime.UtcNow.AddMonths(1),
                    IsActive = true
                });
            }
            else
            {
                existing.ExpiresAtUtc = DateTime.UtcNow.AddMonths(1);
                existing.IsActive = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
