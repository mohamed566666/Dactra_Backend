namespace Dactra.Repositories.Implementation
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription?> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Medicines)
                    .ThenInclude(m => m.DoseTimes)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Slot)
                        .ThenInclude(s => s.Doctor)
                            .ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(pa => pa.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prescription?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medicines)
                    .ThenInclude(m => m.DoseTimes)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Slot)
                        .ThenInclude(s => s.Doctor)
                            .ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                        .ThenInclude(pa => pa.User)
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);
        }

        public async Task<bool> ExistsForAppointmentAsync(int appointmentId)
        {
            return await _context.Prescriptions
                .AnyAsync(p => p.AppointmentId == appointmentId);
        }

        public async Task<Prescription> CreateAsync(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return prescription;
        }

        public async Task UpdateAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _context.PatientAppointments
                .Include(a => a.Slot)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<List<Prescription>> GetByUserIdAsync(string userId)
        {
            return await _context.Prescriptions.Include(p => p.Medicines)
                    .ThenInclude(m => m.DoseTimes)
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                    .ThenInclude(pa => pa.User)
                    .Include(p => p.Appointment)
                    .ThenInclude(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                    .ThenInclude(d => d.User)
                .Where(p => p.Appointment.Patient.User.Id == userId)
                .ToListAsync();
        }
    }
}
