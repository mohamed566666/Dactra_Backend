
using Google;
using Microsoft.EntityFrameworkCore;
using System;

namespace Dactra.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IHubContext<AppointmentHub> _hub;
        private readonly ApplicationDbContext _context;
        public AppointmentService(IHubContext<AppointmentHub> hub, IAppointmentRepository repo,ApplicationDbContext context)
        {
            _hub = hub; 
            _repo = repo;   
            _context = context;
        }
        public async Task<int> BookAppointmentAsync(int patientId, int scheduleTableId)
        {
            var isBooked = await _repo.IsBooked(scheduleTableId);
            if (isBooked)
            {
                throw new Exception("This appointment slot is already booked.");
            }
            var schedule = await _repo.GetScheduleByIdAsync(scheduleTableId);
            if (schedule == null)
                throw new Exception("Schedule not found");


            decimal amount = schedule.Amount;

            var payment = new Payment
            {
                Id = patientId,
                Amount = amount,
                Status = true,
                CreatedAt = DateTime.UtcNow,
                Currency = "EGP",
                Method = "Credit Card"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var appointment = new PatientAppointment
            {
                PatientId = patientId,
                ScheduleTableId = scheduleTableId,
                Status = true,
                BookedAt = DateTime.UtcNow
            };
            await _repo.BookeAsync(appointment);
            await _hub.Clients.Group(scheduleTableId.ToString())
            .SendAsync("AppointmentBooked", new
            {
                ScheduleId = scheduleTableId,
                AppointmentId = appointment.Id
            });
            return appointment.Id;
        }
    }
}
