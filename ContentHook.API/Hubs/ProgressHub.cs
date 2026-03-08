using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace ContentHook.API.Hubs
{
    [Authorize]
    public class ProgressHub : Hub
    {
        public async Task JoinJobGroup(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"job_{jobId}");
        }

        public async Task LeaveJobGroup(string jobId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"job_{jobId}");
        }
    }
}