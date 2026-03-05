using ContentHook.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContentHook.API.Services
{
    public class TempFileVideoStorageService : IVideoStorageService
    {
        private readonly ILogger<TempFileVideoStorageService> _logger;
        private readonly string _tempDir;

        public TempFileVideoStorageService(ILogger<TempFileVideoStorageService> logger)
        {
            _logger = logger;
            _tempDir = Path.Combine(Path.GetTempPath(), "contenthook", "videos");
            Directory.CreateDirectory(_tempDir);
        }

        public async Task<string> SaveAsync(
            Stream videoStream,
            string fileName,
            CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName);
            var storageKey = Path.Combine(_tempDir, $"{Guid.NewGuid()}{extension}");

            await using var fileStream = new FileStream(storageKey, FileMode.Create);
            await videoStream.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation("Video saved: {Key}", storageKey);
            return storageKey;
        }

        public Task<Stream> GetAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            Stream stream = File.OpenRead(storageKey);
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string storageKey)
        {
            try
            {
                if (File.Exists(storageKey))
                {
                    File.Delete(storageKey);
                    _logger.LogInformation("Temp video deleted: {Key}", storageKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not delete temp file: {Key}", storageKey);
            }
            return Task.CompletedTask;
        }
    }
}