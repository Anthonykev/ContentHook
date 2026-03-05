using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IFFmpegService
    {
        Task<Stream> ExtractAudioAsync(
            Stream videoStream,
            CancellationToken cancellationToken = default);
    }
}