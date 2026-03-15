namespace Dactra.Hubs
{
    public class PostHub : Hub
    {
        // ── Connection lifecycle ─────────────────────────────────────────────────

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        // ── Group helpers ────────────────────────────────────────────────────────

        /// <summary>Join the real-time group for a specific post (to receive comments/likes).</summary>
        public async Task JoinPostGroup(int postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, PostGroupName(postId));
        }

        public async Task LeavePostGroup(int postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, PostGroupName(postId));
        }

        // ── Static group name helpers (used by services) ─────────────────────────

        public static string PostGroupName(int postId) => $"post_{postId}";
        public static string GlobalFeedGroup() => "global_feed";
    }
}
