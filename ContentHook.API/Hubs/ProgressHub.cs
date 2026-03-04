using Microsoft.AspNetCore.SignalR;

namespace ContentHook.API.Hubs
{
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