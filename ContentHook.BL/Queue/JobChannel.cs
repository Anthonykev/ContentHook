using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using ContentHook.BL.Interfaces;


namespace ContentHook.BL.Queue
{
    public class JobChannel : IJobQueue
    {
        // BoundedCapacity: max 100 Jobs in der Queue
        // Falls voll: DropWrite — neuer Job wird abgelehnt statt zu blockieren
        private readonly Channel<Guid> _channel = Channel.CreateBounded<Guid>(
            new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,   // nur 1 Worker liest
                SingleWriter = false   // mehrere Controller können schreiben
            });

        public async ValueTask EnqueueAsync(
            Guid jobId,
            CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(jobId, cancellationToken);
        }

        public async IAsyncEnumerable<Guid> ReadAllAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var jobId in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return jobId;
            }
        }
    }

}
