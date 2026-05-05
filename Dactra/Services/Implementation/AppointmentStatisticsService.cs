using Dactra.DTOs.AppointmentDTOs;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Dactra.Services.Implementation
{
    public class AppointmentStatisticsService : IAppointmentStatisticsService
    {
        private readonly IAppointmentStatisticsRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentService _paymentService;
        private readonly IHubContext<AppointmentHub> _hubContext;
        private readonly ILogger<AppointmentStatisticsService> _logger;
        private readonly ApplicationDbContext _context;

        public AppointmentStatisticsService(
            IAppointmentStatisticsRepository repository,
            IAppointmentRepository appointmentRepository,
            IPaymentService paymentService,
            IHubContext<AppointmentHub> hubContext,
            ILogger<AppointmentStatisticsService> logger,
            ApplicationDbContext context)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _paymentService = paymentService;
            _hubContext = hubContext;
            _logger = logger;
            _context = context;
        }

        public async Task<AppointmentStatisticsSummaryDto> GetPatientStatisticsOnlyAsync(int patientId)
        {
            return await _repository.GetPatientStatisticsAsync(patientId);
        }

        public async Task<PagedResultDto<PatientAppointmentListItemDto>> GetPatientAppointmentsPagedAsync(
            int patientId,
            AppointmentFilterRequestDto filter)
        {
            var (appointments, totalCount) = await _repository.GetPatientAppointmentsPagedAsync(patientId, filter);

            var items = appointments.Select(MapToPatientListItemDto).ToList();

            return new PagedResultDto<PatientAppointmentListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<PatientAppointmentsStatsResponse> GetPatientFullStatsAsync(
            int patientId,
            AppointmentFilterRequestDto? filter = null)
        {
            var statistics = await _repository.GetPatientStatisticsAsync(patientId);

            filter ??= new AppointmentFilterRequestDto();
            var (appointments, totalCount) = await _repository.GetPatientAppointmentsPagedAsync(patientId, filter);

            var items = appointments.Select(MapToPatientListItemDto).ToList();

            return new PatientAppointmentsStatsResponse
            {
                Statistics = statistics,
                Appointments = new PagedResultDto<PatientAppointmentListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                }
            };
        }

        public async Task<AppointmentStatisticsSummaryDto> GetDoctorStatisticsOnlyAsync(int doctorId)
        {
            return await _repository.GetDoctorStatisticsAsync(doctorId);
        }

        public async Task<PagedResultDto<DoctorAppointmentListItemDto>> GetDoctorAppointmentsPagedAsync(
            int doctorId,
            AppointmentFilterRequestDto filter)
        {
            var (appointments, totalCount) = await _repository.GetDoctorAppointmentsPagedAsync(doctorId, filter);

            var items = appointments.Select(MapToDoctorListItemDto).ToList();

            return new PagedResultDto<DoctorAppointmentListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<DoctorAppointmentsStatsResponse> GetDoctorFullStatsAsync(
            int doctorId,
            AppointmentFilterRequestDto? filter = null)
        {
            var statistics = await _repository.GetDoctorStatisticsAsync(doctorId);

            filter ??= new AppointmentFilterRequestDto();
            var (appointments, totalCount) = await _repository.GetDoctorAppointmentsPagedAsync(doctorId, filter);

            var items = appointments.Select(MapToDoctorListItemDto).ToList();

            return new DoctorAppointmentsStatsResponse
            {
                Statistics = statistics,
                Appointments = new PagedResultDto<DoctorAppointmentListItemDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize
                }
            };
        }

        public async Task<CancelAppointmentResponseDto> CancelAppointmentAsync(
            int appointmentId,
            int userId,
            string userRole,
            string reason)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var appointment = await _repository.GetAppointmentWithDetailsAsync(appointmentId);

                if (appointment == null)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "Appointment not found",
                        AppointmentId = appointmentId
                    };
                }

                if (userRole == "Patient" && appointment.PatientId != userId)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "You are not authorized to cancel this appointment",
                        AppointmentId = appointmentId
                    };
                }

                if (userRole == "Doctor" && appointment.Slot.DoctorId != userId)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "You are not authorized to cancel this appointment",
                        AppointmentId = appointmentId
                    };
                }

                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "Appointment is already cancelled",
                        AppointmentId = appointmentId,
                        CancelledReason = appointment.CancelledReason
                    };
                }

                if (appointment.Status == AppointmentStatus.Completed)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "Cannot cancel a completed appointment",
                        AppointmentId = appointmentId
                    };
                }

                if (userRole == "Patient")
                {
                    var slotTime = appointment.Slot.SlotDateTimeUtc;
                    var hoursUntilAppointment = (slotTime - DateTime.UtcNow).TotalHours;

                    if (hoursUntilAppointment < 1)
                    {
                        return new CancelAppointmentResponseDto
                        {
                            Success = false,
                            Message = "Appointments can only be cancelled at least 1 hour before the scheduled time",
                            AppointmentId = appointmentId
                        };
                    }
                }

                bool refundProcessed = false;
                decimal? refundAmount = null;

                if (appointment.Payment.Status == paymentStatus.Confirmed &&
                    appointment.Status != AppointmentStatus.Cancelled)
                {
                    try
                    {
                        await _paymentService.RefundAppointmentAsync(appointment.SlotId);
                        refundProcessed = true;
                        refundAmount = appointment.Payment.Amount;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process refund for appointment {AppointmentId}", appointmentId);
                    }
                }

                var cancelled = await _repository.CancelAppointmentWithReasonAsync(appointmentId, reason);

                if (!cancelled)
                {
                    return new CancelAppointmentResponseDto
                    {
                        Success = false,
                        Message = "Failed to cancel appointment",
                        AppointmentId = appointmentId
                    };
                }

                var slot = appointment.Slot;
                slot.IsBooked = false;
                slot.IsReserved = false;
                slot.ReservedUntil = null;
                slot.AppointmentId = null;

                _context.DoctorAvailabilitySlots.Update(slot);

                if (!string.IsNullOrEmpty(appointment.ReminderJobId))
                {
                    BackgroundJob.Delete(appointment.ReminderJobId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _hubContext.Clients.Group($"Doctor_{appointment.Slot.DoctorId}")
                    .SendAsync("AppointmentCancelled", new
                    {
                        AppointmentId = appointment.Id,
                        SlotId = slot.Id,
                        PatientId = appointment.PatientId,
                        PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}".Trim(),
                        SlotDateTime = appointment.Slot.SlotDateTimeUtc,
                        CancelledReason = reason,
                        CancelledBy = userRole,
                        CancelledAt = DateTime.UtcNow
                    });

                await _hubContext.Clients.Group($"Patient_{appointment.PatientId}")
                    .SendAsync("AppointmentCancelled", new
                    {
                        AppointmentId = appointment.Id,
                        SlotId = slot.Id,
                        DoctorId = appointment.Slot.DoctorId,
                        DoctorName = $"{appointment.Slot.Doctor.FirstName} {appointment.Slot.Doctor.LastName}".Trim(),
                        SlotDateTime = appointment.Slot.SlotDateTimeUtc,
                        CancelledReason = reason,
                        CancelledBy = userRole,
                        CancelledAt = DateTime.UtcNow,
                        RefundProcessed = refundProcessed,
                        RefundAmount = refundAmount
                    });

                return new CancelAppointmentResponseDto
                {
                    Success = true,
                    Message = refundProcessed
                        ? "Appointment cancelled and refund processed successfully"
                        : "Appointment cancelled successfully",
                    RefundProcessed = refundProcessed,
                    RefundAmount = refundAmount,
                    AppointmentId = appointment.Id,
                    CancelledReason = reason,
                    CancelledAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling appointment {AppointmentId}", appointmentId);
                return new CancelAppointmentResponseDto
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    AppointmentId = appointmentId
                };
            }
        }

        public async Task<DoctorWeeklyAppointmentsResponseDto> GetDoctorWeeklyAppointmentsByIdAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-6);

            var dailyCounts = await _repository.GetDoctorWeeklyAppointmentsAsync(doctorId);

            return new DoctorWeeklyAppointmentsResponseDto
            {
                DoctorId = doctorId,
                DailyCounts = dailyCounts,
                TotalAppointments = dailyCounts.Sum(d => d.AppointmentCount),
                FromDate = startDate,
                ToDate = today
            };
        }

        public async Task<DoctorWeeklyAppointmentsResponseDto> GetAuthenticatedDoctorWeeklyAppointmentsAsync(int doctorId)
        {
            return await GetDoctorWeeklyAppointmentsByIdAsync(doctorId);
        }

        #region Private Mapping Methods

        private PatientAppointmentListItemDto MapToPatientListItemDto(PatientAppointment appointment)
        {
            var slotUtc = DateTime.SpecifyKind(appointment.Slot.SlotDateTimeUtc, DateTimeKind.Utc);

            var isUpcoming = appointment.Status == AppointmentStatus.Confirmed &&
                            slotUtc > DateTime.UtcNow;

            var statusString = appointment.Status.ToString();
            if (appointment.Status == AppointmentStatus.Confirmed && isUpcoming)
                statusString = "Upcoming";

            return new PatientAppointmentListItemDto
            {
                AppointmentId = appointment.Id,
                DoctorId = appointment.Slot.DoctorId,
                DoctorName = $"{appointment.Slot.Doctor.FirstName} {appointment.Slot.Doctor.LastName}".Trim(),
                DoctorSpecialty = appointment.Slot.Doctor.specialization?.Name ?? "General",
                DoctorImageUrl = appointment.Slot.Doctor.User?.ImageUrl,
                SlotDateTime = slotUtc,
                AppointmentType = appointment.Slot.SlotType.ToString(),
                Status = statusString,
                BookedAt = appointment.BookedAt,
                CancelledReason = appointment.CancelledReason
            };
        }

        private DoctorAppointmentListItemDto MapToDoctorListItemDto(PatientAppointment appointment)
        {
            var slotUtc = DateTime.SpecifyKind(appointment.Slot.SlotDateTimeUtc, DateTimeKind.Utc);

            var isUpcoming = appointment.Status == AppointmentStatus.Confirmed &&
                            slotUtc > DateTime.UtcNow;

            var statusString = appointment.Status.ToString();
            if (appointment.Status == AppointmentStatus.Confirmed && isUpcoming)
                statusString = "Upcoming";

            return new DoctorAppointmentListItemDto
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}".Trim(),
                PatientImageUrl = appointment.Patient.User?.ImageUrl,
                SlotDateTime = slotUtc,
                AppointmentType = appointment.Slot.SlotType.ToString(),
                Status = statusString,
                BookedAt = appointment.BookedAt,
                CancelledReason = appointment.CancelledReason
            };
        }

        #endregion
    }
}