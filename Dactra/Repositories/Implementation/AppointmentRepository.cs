
namespace Dactra.Repositories.Implementation
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PatientAppointment> BookeAsync(PatientAppointment appointment)
        {
         _context.PatientAppointments.Add(appointment);
            await  _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> IsBooked(int scheduletableId)
        {
            var res = await _context.PatientAppointments.AnyAsync(a => a.SlotId == scheduletableId && a.Status == AppointmentStatus.Confirmed);
            return res;
        }
        public async Task<DoctorAvailabilitySlot> GetScheduleByIdAsync(int scheduleTableId)
        {
            return await _context.DoctorAvailabilitySlots
            .Include(s => s.Doctor)
            .FirstOrDefaultAsync(s => s.Id == scheduleTableId);
        }
    }
}
