using Dactra.DTOs.PrescriptionDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> CreatePrescriptionAsync(
            CreatePrescriptionRequestDto dto, int doctorId);

        Task<PrescriptionResponseDto?> GetByAppointmentIdAsync(
            int appointmentId, int userId, string userRole);

        Task<PrescriptionResponseDto?> UpdatePrescriptionAsync(
            int prescriptionId, UpdatePrescriptionRequestDto dto, int doctorId);
    }
}
