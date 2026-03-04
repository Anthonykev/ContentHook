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

        public VideoProcessingWorker(
            IJobQueue jobQueue,
            IServiceScopeFactory scopeFactory,
            ILogger<VideoProcessingWorker> logger)
        {
            _jobQueue = jobQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
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

            // Das muss ich imm Hinterkopf behalten: BackgroundService ist ein Singleton, aber Repository und DbContext sind Scoped.
            using var scope = _scopeFactory.CreateScope();
            var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();

            try
            {
                var job = await jobRepo.GetByIdAsync(jobId);
                if (job is null)
                {
                    _logger.LogWarning("Job {JobId} not found.", jobId);
                    return;
                }

                job.MarkAsTranscribing();
                await jobRepo.UpdateAsync(job);
                _logger.LogInformation("Job {JobId} → Transcribing", jobId);

                // TODO: echter Whisper-Call kommt dann hier
                await Task.Delay(2000, cancellationToken); // simuliert Whisper-Dauer


                var fakeTranscriptId = Guid.NewGuid(); // TODO : echter Transcript noch einbauen
                job.MarkAsGenerating(fakeTranscriptId);
                await jobRepo.UpdateAsync(job);
                _logger.LogInformation("Job {JobId} → Generating", jobId);

                // TODO: echter GPT-Call kommt hier
                await Task.Delay(2000, cancellationToken); // simuliert GPT-Dauer

                
                var fakeGenerationId = Guid.NewGuid(); // TODO: echte Generation mach ich dann hier
                job.MarkAsDone(fakeGenerationId);
                await jobRepo.UpdateAsync(job);
                _logger.LogInformation("Job {JobId} → Done", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job {JobId} failed.", jobId);

                using var errorScope = _scopeFactory.CreateScope();
                var errorJobRepo = errorScope.ServiceProvider.GetRequiredService<IJobRepository>();

                var job = await errorJobRepo.GetByIdAsync(jobId);
                if (job is not null)
                {
                    job.MarkAsFailed(ex.Message);
                    await errorJobRepo.UpdateAsync(job);
                }
            }
        }
    }
}
