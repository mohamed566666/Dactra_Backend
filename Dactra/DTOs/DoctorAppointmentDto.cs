namespace Dactra.DTOs
{
    public record DoctorAppointmentDto(
        int AppointmentId,
        string PatientName,
        string PatientEmail,
        DateTime SlotDateTime,
        string SlotType,
        string Status,
        DateTime BookedAt,
        decimal Amount,
        string PaymentStatus
    );
}
