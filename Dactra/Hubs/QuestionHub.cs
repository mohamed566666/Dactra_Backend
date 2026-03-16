namespace Dactra.Hubs
{
    [Authorize]
    public class QuestionHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GlobalFeedGroup());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinQuestionGroup(int questionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, QuestionGroupName(questionId));
        }

        public async Task LeaveQuestionGroup(int questionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, QuestionGroupName(questionId));
        }

        public static string QuestionGroupName(int questionId) => $"question_{questionId}";
        public static string GlobalFeedGroup() => "global_questions_feed";
        public static string UserGroup(string userId) => $"user_{userId}";
    }
}
