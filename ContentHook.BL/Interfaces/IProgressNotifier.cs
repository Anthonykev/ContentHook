using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IProgressNotifier
    {
        Task NotifyAsync(Guid jobId, string status, object? payload = null);
    }
}