using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dactra.Hubs
{
    [Authorize]
    public class QuestionsHub:Hub
    {
    }
}
