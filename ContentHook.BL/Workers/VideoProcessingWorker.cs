using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.BL.Interfaces;
using ContentHook.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContentHook.BL.Workers
{
    public class VideoProcessingWorker : BackgroundService
    {
        private readonly IJobQueue _jobQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<VideoProcessingWorker> _logger;
        private readonly IProgressNotifier _notifier;

        public VideoProcessingWorker(
            IJobQueue jobQueue,
            IServiceScopeFactory scopeFactory,
            ILogger<VideoProcessingWorker> logger,
            IProgressNotifier notifier)
        {
            _jobQueue = jobQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _notifier = notifier;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VideoProcessingWorker started.");

            await foreach (var jobId in _jobQueue.ReadAllAsync(stoppingToken))
            {
                await ProcessJobAsync(jobId, stoppingToken);
            }
            _logger.LogInformation("VideoProcessingWorker stopped.");
        }

        private async Task ProcessJobAsync(Guid jobId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing job {JobId}", jobId);

            using var scope = _scopeFactory.CreateScope();
            var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
            var storage = scope.ServiceProvider.GetRequiredService<IVideoStorageService>();
            var ffmpeg = scope.ServiceProvider.GetRequiredService<IFFmpegService>();
            var whisper = scope.ServiceProvider.GetRequiredService<ITranscriptionService>();
            var transcriptService = scope.ServiceProvider.GetRequiredService<ITranscriptService>();

            try
            {
                var job = await jobRepo.GetByIdAsync(jobId);
                if (job is null)
                {
                    _logger.LogWarning("Job {JobId} not found.", jobId);
                    return;
                }

                // Transcribing 
                job.MarkAsTranscribing();
                await jobRepo.UpdateAsync(job);
                await _notifier.NotifyAsync(jobId, "transcribing");

                // await using → Streams werden sauber disposed
                await using var videoStream = await storage.GetAsync(
                    job.VideoStorageKey, cancellationToken);

                await using var audioStream = await ffmpeg.ExtractAudioAsync(
                    videoStream, cancellationToken);

                var transcriptText = await whisper.TranscribeAsync(
                    audioStream, "de", cancellationToken);

                // Video nach Transkription löschen
                await storage.DeleteAsync(job.VideoStorageKey);

                // Transcript in DB speichern
                var transcript = await transcriptService.CreateAsync(
                    job.UserId,
                    transcriptText,
                    language: "de",
                    originalFileName: job.OriginalFileName);

                _logger.LogInformation("Job {JobId} → Transcribed: {Id}", jobId, transcript.Id);

                // Generating 
                job.MarkAsGenerating(transcript.Id);
                await jobRepo.UpdateAsync(job);
                await _notifier.NotifyAsync(jobId, "generating");

                // TODO: echter GPT-Call mit Fortschritts-Updates implementieren
                await Task.Delay(2000, cancellationToken);

                // Done 
                var fakeGenerationId = Guid.NewGuid();
                job.MarkAsDone(fakeGenerationId);
                await jobRepo.UpdateAsync(job);
                await _notifier.NotifyAsync(jobId, "done", new
                {
                    transcriptId = transcript.Id,
                    generationId = fakeGenerationId
                });

                _logger.LogInformation("Job {JobId} → Done", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job {JobId} failed.", jobId);
                await _notifier.NotifyAsync(jobId, "failed", new { error = ex.Message });

                using var errorScope = _scopeFactory.CreateScope();
                var errorJobRepo = errorScope.ServiceProvider.GetRequiredService<IJobRepository>();
                var failedJob = await errorJobRepo.GetByIdAsync(jobId);
                if (failedJob is not null)
                {
                    failedJob.MarkAsFailed(ex.Message);
                    await errorJobRepo.UpdateAsync(failedJob);
                }
            }
        }
    }
}