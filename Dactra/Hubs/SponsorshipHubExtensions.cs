using Dactra.DTOs.Sponsorship;
using Microsoft.AspNetCore.SignalR;

namespace Dactra.Hubs
{
    public static class SponsorshipHubExtensions
    {
        public static Task NotifyOfferReceived(
            this IHubContext<SponsorshipHub> hub,
            int doctorId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"doctor_{doctorId}")
                  .SendAsync("OfferReceived", dto);

        public static Task NotifyOfferAccepted(
            this IHubContext<SponsorshipHub> hub,
            int providerId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"provider_{providerId}")
                  .SendAsync("OfferAccepted", dto);
        public static Task NotifyOfferRejected(
            this IHubContext<SponsorshipHub> hub,
            int providerId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"provider_{providerId}")
                  .SendAsync("OfferRejected", dto);

        public static Task NotifyCounterOfferReceived(
            this IHubContext<SponsorshipHub> hub,
            int providerId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"provider_{providerId}")
                  .SendAsync("CounterOfferReceived", dto);

        public static Task NotifyCounterAccepted(
            this IHubContext<SponsorshipHub> hub,
            int doctorId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"doctor_{doctorId}")
                  .SendAsync("CounterAccepted", dto);

        public static Task NotifyCounterRejected(
            this IHubContext<SponsorshipHub> hub,
            int doctorId,
            SponsorshipResponseDTO dto)
            => hub.Clients
                  .Group($"doctor_{doctorId}")
                  .SendAsync("CounterRejected", dto);

        public static async Task NotifySponsorshipCancelled(
            this IHubContext<SponsorshipHub> hub,
            int providerId,
            int doctorId,
            SponsorshipResponseDTO dto)
        {
            await hub.Clients
                     .Group($"provider_{providerId}")
                     .SendAsync("SponsorshipCancelled", dto);

            await hub.Clients
                     .Group($"doctor_{doctorId}")
                     .SendAsync("SponsorshipCancelled", dto);
        }
    }
}