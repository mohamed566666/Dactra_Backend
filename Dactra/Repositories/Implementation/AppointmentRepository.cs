
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
           var res=await _context.PatientAppointments.AnyAsync(a => a.ScheduleTableId == scheduletableId && a.Status==true);
            return res;
        }
        public async Task<ScheduleTable> GetScheduleByIdAsync(int scheduleTableId)
        {
            return await _context.ScheduleTables
            .Include(s => s.Doctor)
            .FirstOrDefaultAsync(s => s.Id == scheduleTableId);
        }
    }
}
