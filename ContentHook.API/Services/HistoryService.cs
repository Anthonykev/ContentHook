using ContentHook.API.DTOs;
using ContentHook.DAL.Interfaces;

namespace ContentHook.API.Services
{
    public class HistoryService
    {
        private readonly IJobRepository _jobRepo;
        private readonly ITranscriptRepository _transcriptRepo;
        private readonly IGenerationRepository _generationRepo;

        public HistoryService(
            IJobRepository jobRepo,
            ITranscriptRepository transcriptRepo,
            IGenerationRepository generationRepo)
        {
            _jobRepo = jobRepo;
            _transcriptRepo = transcriptRepo;
            _generationRepo = generationRepo;
        }

        public async Task<List<HistoryItemDto>> GetAllByUserAsync(string userId)
        {
            var jobs = await _jobRepo.GetAllByUserAsync(userId);

            var transcriptIds = jobs
                .Where(j => j.TranscriptId.HasValue)
                .Select(j => j.TranscriptId!.Value)
                .Distinct()
                .ToList();

            var transcripts = new Dictionary<Guid, string>();
            foreach (var tid in transcriptIds)
            {
                var t = await _transcriptRepo.GetByIdAsync(tid);
                if (t is not null)
                    transcripts[tid] = t.Text;
            }

          
            var generations = new Dictionary<Guid, List<GenerationResponseDto>>();
            foreach (var tid in transcriptIds)
            {
                var gens = await _generationRepo.GetByTranscriptIdForUserAsync(tid, userId);
                generations[tid] = gens
                    .Select(g => new GenerationResponseDto(
                        g.Id, g.Platform, g.Title, g.Hook, g.Hashtags,
                        g.Tonality, g.ModelUsed, g.PromptVersion,
                        g.RegenerationIndex, g.CreatedAt))
                    .ToList();
            }

            return jobs.Select(job => new HistoryItemDto(
                job.Id,
                job.OriginalFileName,
                job.Platform,
                job.Status.ToString(),
                job.TranscriptId,
                job.TranscriptId.HasValue
                    ? transcripts.GetValueOrDefault(job.TranscriptId.Value)
                    : null,
                job.CreatedAt,
                job.TranscriptId.HasValue
                    ? generations.GetValueOrDefault(job.TranscriptId.Value, [])
                    : []
            )).ToList();
        }

        public async Task<HistoryItemDto?> GetByJobIdAsync(string userId, Guid jobId)
        {
            var job = await _jobRepo.GetByIdAsync(jobId);


            if (job is null || job.UserId != userId)
                return null;

            string? transcriptText = null;
            var generationDtos = new List<GenerationResponseDto>();

            if (job.TranscriptId.HasValue)
            {
                var transcript = await _transcriptRepo.GetByIdAsync(job.TranscriptId.Value);
                transcriptText = transcript?.Text;

                var gens = await _generationRepo
                    .GetByTranscriptIdForUserAsync(job.TranscriptId.Value, userId);

                generationDtos = gens
                    .Select(g => new GenerationResponseDto(
                        g.Id, g.Platform, g.Title, g.Hook, g.Hashtags,
                        g.Tonality, g.ModelUsed, g.PromptVersion,
                        g.RegenerationIndex, g.CreatedAt))
                    .ToList();
            }

            return new HistoryItemDto(
                job.Id,
                job.OriginalFileName,
                job.Platform,
                job.Status.ToString(),
                job.TranscriptId,
                transcriptText,
                job.CreatedAt,
                generationDtos
            );
        }
    }
}