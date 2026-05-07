using Dactra.DTOs.PrescriptionDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorProfileRepository _doctorRepo;
        private readonly IPatientProfileRepository _patientRepo;
        private readonly IMedicineReminderService _reminderService;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IDoctorProfileRepository doctorRepo,
            IPatientProfileRepository patientRepo, IMedicineReminderService medicineReminderService)
        {
            _prescriptionService = prescriptionService;
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _reminderService = medicineReminderService;
        }

        #region Create Prescription

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionRequestDto dto)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null)
                    return Unauthorized(new { message = "Doctor profile not found" });
                var exists = await _prescriptionService.GetByAppointmentIdAsync(dto.AppointmentId, doctor.Id, "Doctor");
                var reminders =0;
                if (exists != null)
                {
                    var updateDto = new UpdatePrescriptionRequestDto
                    {
                        Diagnosis = dto.Diagnosis,
                        Medicines = dto.Medicines
                    };
                    var updated = await _prescriptionService.UpdatePrescriptionAsync(exists.Id, updateDto, doctor.Id);

                     reminders = await _reminderService.CreateFromPrescriptionAsync(dto.AppointmentId);
                    return Ok(new {
                        updated= updated,
                        reminderscount=reminders
                    
                    });
                }
                var result = await _prescriptionService.CreatePrescriptionAsync(dto, doctor.Id);
                 reminders = await _reminderService.CreateFromPrescriptionAsync(dto.AppointmentId);
                return CreatedAtAction(
                    nameof(GetByAppointment),
                    new { appointmentId = dto.AppointmentId },
                    new {
                       result= result ,
                       remindercount =reminders
                    });
            }
            catch (UnauthorizedAccessException ex) {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
            }
        }

        #endregion

        #region Get Prescription By Appointment

        [HttpGet("appointment/{appointmentId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            try
            {
                var (userId, userRole) = GetUserInfo();

                int profileId;
                if (userRole == "Doctor")
                {
                    var doctor = await GetDoctorAsync();
                    if (doctor is null)
                        return Unauthorized(new { message = "Doctor profile not found" });
                    profileId = doctor.Id;
                }
                else
                {
                    var patient = await GetPatientAsync();
                    if (patient is null)
                        return Unauthorized(new { message = "Patient profile not found" });
                    profileId = patient.Id;
                }

                var result = await _prescriptionService.GetByAppointmentIdAsync(appointmentId, profileId, userRole);

                if (result is null)
                    return NotFound(new { message = "No prescription found for this appointment" });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
            }
        }

        #endregion

        #region Update Prescription

        [HttpPut("{prescriptionId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(int prescriptionId, [FromBody] UpdatePrescriptionRequestDto dto)
        {
            try
            {
                var doctor = await GetDoctorAsync();
                if (doctor is null)
                    return Unauthorized(new { message = "Doctor profile not found" });

                var result = await _prescriptionService.UpdatePrescriptionAsync(prescriptionId, dto, doctor.Id);

                if (result is null)
                    return NotFound(new { message = "Prescription not found" });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", detail = ex.Message });
            }
        }

        #endregion

        #region Helpers

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private string UserRole => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        private (string userId, string role) GetUserInfo() => (UserId, UserRole);

        private async Task<DoctorProfile?> GetDoctorAsync()
            => await _doctorRepo.GetByUserIdAsync(UserId);

        private async Task<PatientProfile?> GetPatientAsync()
            => await _patientRepo.GetByUserIdAsync(UserId);

        #endregion
    }
}
