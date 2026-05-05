// Dactra/Controllers/AppointmentStatisticsController.cs
using Dactra.DTOs.AppointmentDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentStatisticsController : ControllerBase
    {
        private readonly IAppointmentStatisticsService _statisticsService;
        private readonly IPatientProfileRepository _patientRepo;
        private readonly IDoctorProfileRepository _doctorRepo;

        public AppointmentStatisticsController(
            IAppointmentStatisticsService statisticsService,
            IPatientProfileRepository patientRepo,
            IDoctorProfileRepository doctorRepo)
        {
            _statisticsService = statisticsService;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        #region Statistics Only

        [HttpGet("statistics")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetStatisticsOnly()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Patient")
            {
                var patient = await GetPatientAsync();
                if (patient is null)
                    return Unauthorized(new { message = "Patient profile not found" });

                var result = await _statisticsService.GetPatientStatisticsOnlyAsync(patient.Id);
                return Ok(result);
            }
            else if (userRole == "Doctor")
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null)
                    return Unauthorized(new { message = "Doctor profile not found" });

                var result = await _statisticsService.GetDoctorStatisticsOnlyAsync(doctor.Id);
                return Ok(result);
            }

            return Unauthorized(new { message = "Invalid role" });
        }

        #endregion

        #region Appointments Only (Paged with Status)

        [HttpGet("appointments")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetAppointmentsPaged(
    [FromQuery] AppointmentStatus status,[FromQuery] SlotType? type = null,[FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,[FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            bool isUpcoming = status == AppointmentStatus.Confirmed;

            var filter = new AppointmentFilterRequestDto
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                Type = type,
                FromDate = fromDate,
                ToDate = toDate,
                UpcomingOnly = isUpcoming
            };

            if (userRole == "Patient")
            {
                var patient = await GetPatientAsync();
                if (patient is null)
                    return Unauthorized(new { message = "Patient profile not found" });

                var result = await _statisticsService.GetPatientAppointmentsPagedAsync(patient.Id, filter);
                return Ok(result);
            }
            else if (userRole == "Doctor")
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null)
                    return Unauthorized(new { message = "Doctor profile not found" });

                var result = await _statisticsService.GetDoctorAppointmentsPagedAsync(doctor.Id, filter);
                return Ok(result);
            }

            return Unauthorized(new { message = "Invalid role" });
        }

        [HttpGet("doctor/{doctorId}/weekly-appointments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorWeeklyAppointments(int doctorId)
        {
            var result = await _statisticsService.GetDoctorWeeklyAppointmentsByIdAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("my-weekly-appointments")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyWeeklyAppointments()
        {
            var doctor = await GetDoctorAsync();
            if (doctor is null)
                return Unauthorized(new { message = "Doctor profile not found" });

            var result = await _statisticsService.GetAuthenticatedDoctorWeeklyAppointmentsAsync(doctor.Id);
            return Ok(result);
        }

        #endregion

        #region Helpers

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private async Task<PatientProfile?> GetPatientAsync()
            => await _patientRepo.GetByUserIdAsync(UserId);

        private async Task<DoctorProfile?> GetDoctorAsync()
            => await _doctorRepo.GetByUserIdAsync(UserId);

        #endregion
    }
}