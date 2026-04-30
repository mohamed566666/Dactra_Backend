using Dactra.DTOs.AppointmentDTOs;

namespace Dactra.Repositories.Implementation
{
    public class AppointmentStatisticsRepository : IAppointmentStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsAsync(int patientId)
        {
            var now = DateTime.UtcNow;

            // استعلام واحد لجلب كل الإحصائيات دفعة واحدة
            var appointments = await _context.PatientAppointments
                .Include(a => a.Slot)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == patientId)
                .Select(a => new
                {
                    a.Status,
                    SlotDateTime = a.Slot.SlotDateTimeUtc,
                    PaymentStatus = a.Payment.Status
                })
                .ToListAsync();

            var completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);
            var upcoming = appointments.Count(a => a.Status == AppointmentStatus.Confirmed && a.SlotDateTime > now);
            var unpaid = appointments.Count(a => a.Status == AppointmentStatus.Pending && a.PaymentStatus == paymentStatus.Pending);

            return new AppointmentStatisticsSummaryDto
            {
                Completed = completed,
                Upcoming = upcoming,
                Cancelled = cancelled,
                Unpaid = unpaid,
                Total = completed + upcoming + cancelled + unpaid
            };
        }

        public async Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsAsync(int doctorId)
        {
            var now = DateTime.UtcNow;

            var appointments = await _context.PatientAppointments
                .Include(a => a.Slot)
                .Include(a => a.Payment)
                .Where(a => a.Slot.DoctorId == doctorId)
                .Select(a => new
                {
                    a.Status,
                    SlotDateTime = a.Slot.SlotDateTimeUtc,
                    PaymentStatus = a.Payment.Status
                })
                .ToListAsync();

            var completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);
            var upcoming = appointments.Count(a => a.Status == AppointmentStatus.Confirmed && a.SlotDateTime > now);
            var unpaid = appointments.Count(a => a.Status == AppointmentStatus.Pending && a.PaymentStatus == paymentStatus.Pending);

            return new AppointmentStatisticsSummaryDto
            {
                Completed = completed,
                Upcoming = upcoming,
                Cancelled = cancelled,
                Unpaid = unpaid,
                Total = completed + upcoming + cancelled + unpaid
            };
        }

        public async Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetPatientAppointmentsPagedAsync(
            int patientId,
            AppointmentFilterRequestDto filter)
        {
            var query = _context.PatientAppointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                        .ThenInclude(d => d.User)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                        .ThenInclude(d => d.specialization)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == patientId)
                .AsQueryable();

            // تطبيق الفلاتر فقط على البيانات المفهرسة
            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(a => a.Slot.SlotType == filter.Type.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(a => a.Slot.SlotDateTimeUtc >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(a => a.Slot.SlotDateTimeUtc <= filter.ToDate.Value);
            }

            var totalCount = await query.CountAsync();

            query = query.OrderByDescending(a => a.Slot.SlotDateTimeUtc);

            var appointments = await query
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            return (appointments, totalCount);
        }

        public async Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)> GetDoctorAppointmentsPagedAsync(
            int doctorId,
            AppointmentFilterRequestDto filter)
        {
            var query = _context.PatientAppointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Payment)
                .Where(a => a.Slot.DoctorId == doctorId)
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(a => a.Slot.SlotType == filter.Type.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(a => a.Slot.SlotDateTimeUtc >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(a => a.Slot.SlotDateTimeUtc <= filter.ToDate.Value);
            }

            var totalCount = await query.CountAsync();

            query = query.OrderByDescending(a => a.Slot.SlotDateTimeUtc);

            var appointments = await query
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            return (appointments, totalCount);
        }

        public async Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _context.PatientAppointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                        .ThenInclude(d => d.User)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                        .ThenInclude(d => d.specialization)
                .Include(a => a.Payment)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<bool> CancelAppointmentWithReasonAsync(int appointmentId, string reason)
        {
            var appointment = await _context.PatientAppointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null || appointment.Status == AppointmentStatus.Cancelled)
                return false;

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledReason = reason;

            return true;
        }
    }
}
