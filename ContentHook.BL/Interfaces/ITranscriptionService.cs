using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface ITranscriptionService
    {
        Task<string> TranscribeAsync(
            Stream audioStream,
            string language,
            CancellationToken cancellationToken = default);
    }
}