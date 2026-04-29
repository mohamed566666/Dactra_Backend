using Dactra.DTOs.VideoCallDTOs;

namespace Dactra.Services.Interfaces
{
    public interface IVideoCallService
    {
        Task<JoinRoomResponseDto> JoinRoomAsync(int appointmentId, string userId);
        Task<RoomStatusDto> GetRoomStatusAsync(int appointmentId);
        Task EndCallAsync(int appointmentId, string userId);
    }
}
