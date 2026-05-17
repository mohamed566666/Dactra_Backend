namespace Dactra.DTOs.VideoCallDTOs
{
    public record JoinRoomResponseDto(
         string RoomName,
         string JitsiToken,
         string JitsiDomain,
         string DisplayName,
         string Role, 
         int SessionId
     );

    public record RoomStatusDto(
        int SessionId,
        string RoomName,
        VideoCallStatus Status,
        bool IsDoctorOnline,
        bool IsPatientOnline,
        DateTime? StartedAtUtc,
        int PatientId
    );
    public record EndCallRequestDto(int AppointmentId);
}
