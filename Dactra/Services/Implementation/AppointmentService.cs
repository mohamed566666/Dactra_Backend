
using Microsoft.EntityFrameworkCore;

namespace Dactra.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IHubContext<AppointmentHub> _hub;
        public AppointmentService(IHubContext<AppointmentHub> hub, IAppointmentRepository repo)
        {
            _hub = hub; 
            _repo = repo;   
        }
        public async Task<int> BookAppointmentAsync(int patientId, int scheduleTableId)
        {
            var isBooked = await _repo.IsBooked(scheduleTableId);
            if (isBooked)
            {
                throw new Exception("This appointment slot is already booked.");
            }

            //var payment = new Payment
            //{
            //    Id = patientId,
            //    Amount = 200,
            //    Status =true,
            //    CreatedAt = DateTime.UtcNow,
            //    Currency = "USD",
            //    Method = "Credit Card"
            //};

            //_context.Payments.Add(payment);
            //await _context.SaveChangesAsync();

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
