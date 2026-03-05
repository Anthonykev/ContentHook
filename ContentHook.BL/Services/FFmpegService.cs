using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ContentHook.BL.Services
{
    public class FFmpegService : IFFmpegService
    {
        private readonly ILogger<FFmpegService> _logger;

        public FFmpegService(ILogger<FFmpegService> logger)
        {
            _logger = logger;
        }

        public async Task<Stream> ExtractAudioAsync(
            Stream videoStream,
            CancellationToken cancellationToken = default)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "contenthook_ffmpeg");
            Directory.CreateDirectory(tempDir);

            var tempVideoPath = Path.Combine(tempDir, $"{Guid.NewGuid()}.mp4");
            var tempAudioPath = Path.Combine(tempDir, $"{Guid.NewGuid()}.mp3");

            try
            {
                
                await using (var fs = File.Create(tempVideoPath))
                    await videoStream.CopyToAsync(fs, cancellationToken);
                
                var args = $"-hide_banner -loglevel error -i \"{tempVideoPath}\" " +
                           $"-vn -ar 16000 -ac 1 -b:a 64k \"{tempAudioPath}\" -y";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = args,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var error = await process.StandardError.ReadToEndAsync(cancellationToken);
                await process.WaitForExitAsync(cancellationToken);

                if (process.ExitCode != 0)
                {
                    _logger.LogError("FFmpeg failed: {Error}", error);
                    throw new InvalidOperationException($"FFmpeg failed: {error}");
                }

                var audioBytes = await File.ReadAllBytesAsync(tempAudioPath, cancellationToken);
                _logger.LogInformation("Audio extracted, size: {Size} bytes", audioBytes.Length);
                return new MemoryStream(audioBytes);
            }
            finally
            {
                if (File.Exists(tempVideoPath)) File.Delete(tempVideoPath);
                if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
            }
        }
    }
}