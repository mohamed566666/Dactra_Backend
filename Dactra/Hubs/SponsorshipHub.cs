using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Dactra.Hubs
{
    [Authorize]
    public class SponsorshipHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = Context.User!.FindFirstValue(ClaimTypes.Role)!;

            var groupName = userRole switch
            {
                "MedicalTestProvider" => $"provider_{userId}",
                "Doctor" => $"doctor_{userId}",
                _ => null
            };

            if (groupName is not null)
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

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