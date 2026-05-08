using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Dactra.Hubs
{
    [Authorize]
    public class SponsorshipHub : Hub
    {
        private readonly IMedicalTestProviderProfileRepository _providerRepo;
        private readonly IDoctorProfileRepository _doctorRepo;

        public SponsorshipHub(
            IMedicalTestProviderProfileRepository providerRepo,
            IDoctorProfileRepository doctorRepo)
        {
            _providerRepo = providerRepo;
            _doctorRepo = doctorRepo;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = Context.User!.FindFirstValue(ClaimTypes.Role)!;

            if (userRole == "MedicalTestProvider")
            {
                var provider = await _providerRepo.GetByUserIdAsync(userId);
                if (provider is not null)
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"provider_{provider.Id}");
            }
            else if (userRole == "Doctor")
            {
                var doctor = await _doctorRepo.GetByUserIdAsync(userId);
                if (doctor is not null)
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"doctor_{doctor.Id}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinSponsorshipRoom(int sponsorshipId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"sponsorship_{sponsorshipId}");
        }

        public async Task LeaveSponsorshipRoom(int sponsorshipId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"sponsorship_{sponsorshipId}");
        }
    }
}