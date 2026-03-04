using ContentHook.API.Hubs;
using ContentHook.BL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ContentHook.API.Services
{
    public class SignalRProgressNotifier : IProgressNotifier
    {
        private readonly IHubContext<ProgressHub> _hubContext;

        public SignalRProgressNotifier(IHubContext<ProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAsync(Guid jobId, string status, object? payload = null)
        {
            await _hubContext.Clients
                .Group($"job_{jobId}")
                .SendAsync("JobStatus", new { jobId, status, payload });
        }
    }
}