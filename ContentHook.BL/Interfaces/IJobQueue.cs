using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(Guid jobId, CancellationToken cancellationToken = default);
        IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken);
    }
}
