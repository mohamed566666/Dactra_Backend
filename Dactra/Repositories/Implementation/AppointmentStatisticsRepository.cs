using Dactra.DTOs.AppointmentDTOs;
using Dactra.Enums;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class AppointmentStatisticsRepository : IAppointmentStatisticsRepository
    {
        private readonly ApplicationDbContext _context;

        private const int DefaultDurationMinutes = 30;

        public AppointmentStatisticsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private static int GetDuration(PatientAppointment a)
        {
            if (a.Slot.SlotType == SlotType.Online)
                return a.Slot.Doctor.OnlineConsultationDurationMinutes ?? DefaultDurationMinutes;

            return a.Slot.Doctor.ConsultationDurationMinutes ?? DefaultDurationMinutes;
        }

        private static bool IsSessionOver(PatientAppointment a, DateTime now)
        {
            if (a.Slot.SlotType != SlotType.Online)
                return false;

            var duration = GetDuration(a);
            return a.Slot.SlotDateTimeUtc.AddMinutes(duration) < now;
        }

        // ─── Statistics ───────────────────────────────────────────────────────

        public async Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsAsync(int patientId)
        {
            var now = DateTime.UtcNow;

            var appointments = await _context.PatientAppointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            return BuildSummary(appointments, now);
        }

        public async Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsAsync(int doctorId)
        {
            var now = DateTime.UtcNow;

            var appointments = await _context.PatientAppointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Where(a => a.Slot.DoctorId == doctorId)
                .ToListAsync();

            return BuildSummary(appointments, now);
        }

        private static AppointmentStatisticsSummaryDto BuildSummary(
            List<PatientAppointment> appointments, DateTime now)
        {
            var completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);
            var failed = appointments.Count(a => a.Status == AppointmentStatus.Failed);

            var upcoming = appointments.Count(a =>
                a.Status == AppointmentStatus.Confirmed && !IsSessionOver(a, now) &&
                a.Slot.SlotDateTimeUtc > now);

            var unpaid = appointments.Count(a =>
                a.Status == AppointmentStatus.Pending &&
                a.Payment.Status == paymentStatus.Pending &&
                !IsSessionOver(a, now));

            return new AppointmentStatisticsSummaryDto
            {
                Completed = completed,
                Upcoming = upcoming,
                Cancelled = cancelled,
                Failed = failed,
                Unpaid = unpaid,
                Total = completed + upcoming + cancelled + failed + unpaid
            };
        }

        public async Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)>
            GetPatientAppointmentsPagedAsync(int patientId, AppointmentFilterRequestDto filter)
        {
            var now = DateTime.UtcNow;

            var query = _context.PatientAppointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Slot).ThenInclude(s => s.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Slot).ThenInclude(s => s.Doctor).ThenInclude(d => d.specialization)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == patientId)
                .AsQueryable();

            query = ApplyCommonFilters(query, filter, now);

            var totalCount = await query.CountAsync();

            query = filter.UpcomingOnly
                ? query.OrderBy(a => a.Slot.SlotDateTimeUtc)
                : query.OrderByDescending(a => a.Slot.SlotDateTimeUtc);

            var appointments = await query
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            if (filter.UpcomingOnly)
                appointments = appointments.Where(a => !IsSessionOver(a, now)).ToList();

            return (appointments, totalCount);
        }


        public async Task<(IEnumerable<PatientAppointment> Appointments, int TotalCount)>
            GetDoctorAppointmentsPagedAsync(int doctorId, AppointmentFilterRequestDto filter)
        {
            var now = DateTime.UtcNow;

            var query = _context.PatientAppointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Slot).ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Where(a => a.Slot.DoctorId == doctorId)
                .AsQueryable();

            query = ApplyCommonFilters(query, filter, now);

            var totalCount = await query.CountAsync();

            query = filter.UpcomingOnly
                ? query.OrderBy(a => a.Slot.SlotDateTimeUtc)
                : query.OrderByDescending(a => a.Slot.SlotDateTimeUtc);

            var appointments = await query
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            if (filter.UpcomingOnly)
                appointments = appointments.Where(a => !IsSessionOver(a, now)).ToList();

            return (appointments, totalCount);
        }

        private static IQueryable<PatientAppointment> ApplyCommonFilters(
            IQueryable<PatientAppointment> query,
            AppointmentFilterRequestDto filter,
            DateTime now)
        {
            if (filter.Status.HasValue)
                query = query.Where(a => a.Status == filter.Status.Value);

            if (filter.UpcomingOnly)
                query = query.Where(a =>
                    a.Status == AppointmentStatus.Confirmed &&
                    a.Slot.SlotDateTimeUtc > now);

            if (filter.Type.HasValue)
                query = query.Where(a => a.Slot.SlotType == filter.Type.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.Slot.SlotDateTimeUtc >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(a => a.Slot.SlotDateTimeUtc <= filter.ToDate.Value);

            return query;
        }

        public async Task<PatientAppointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _context.PatientAppointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Slot).ThenInclude(s => s.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Slot).ThenInclude(s => s.Doctor).ThenInclude(d => d.specialization)
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


        public async Task<List<DoctorDailyAppointmentsDto>> GetDoctorWeeklyAppointmentsAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-6);

            var slotDates = await _context.PatientAppointments
                .Where(a =>
                    a.Slot.DoctorId == doctorId &&
                    a.Status == AppointmentStatus.Completed &&
                    a.Slot.SlotDateTimeUtc >= startDate &&
                    a.Slot.SlotDateTimeUtc < today.AddDays(1))
                .Select(a => a.Slot.SlotDateTimeUtc)
                .ToListAsync();

            var grouped = slotDates
                .GroupBy(d => d.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = startDate.AddDays(offset);
                    return new DoctorDailyAppointmentsDto
                    {
                        Date = date,
                        DayName = date.DayOfWeek.ToString(),
                        AppointmentCount = grouped.TryGetValue(date, out var count) ? count : 0
                    };
                })
                .ToList();
        }


        public async Task<List<PatientAppointment>> GetExpiredOnlineAppointmentsAsync()
        {
            var now = DateTime.UtcNow;

            var candidates = await _context.PatientAppointments
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Doctor)
                .Include(a => a.Payment)
                .Where(a =>
                    a.Slot.SlotType == SlotType.Online &&
                    (a.Status == AppointmentStatus.Confirmed ||
                     a.Status == AppointmentStatus.Pending) &&
                    a.Slot.SlotDateTimeUtc < now)
                .ToListAsync();

            return candidates
                .Where(a => IsSessionOver(a, now))
                .ToList();
        }
    }
}