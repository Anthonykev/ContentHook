using ContentHook.API.DTOs;
using ContentHook.BL.Interfaces;
using ContentHook.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IJobRepository _jobRepo;
        private readonly ITranscriptService _transcriptService;
        private readonly IGenerationService _generationService;
        private readonly IProgressNotifier _notifier;
        private readonly IGenerationRepository _generationRepo;

        public JobsController(
            IJobRepository jobRepo,
            ITranscriptService transcriptService,
            IGenerationService generationService,
            IProgressNotifier notifier,
            IGenerationRepository generationRepo)
        {
            _jobRepo = jobRepo;
            _transcriptService = transcriptService;
            _generationService = generationService;
            _notifier = notifier;
            _generationRepo = generationRepo;
        }

        [HttpPost("{id:guid}/generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Generate(
            Guid id,
            [FromBody] StartGenerationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var job = await _jobRepo.GetByIdAsync(id);
            if (job is null || job.UserId != userId)
                return NotFound();

            if (job.Status != ContentHook.DAL.Entities.JobStatus.Transcribed &&
                job.Status != ContentHook.DAL.Entities.JobStatus.Done)
            {
                return BadRequest($"Job is in status '{job.Status}' — expected 'Transcribed' or 'Done'.");
            }

            if (!job.TranscriptId.HasValue)
                return BadRequest("Job has no transcript.");

            var transcript = await _transcriptService.GetByIdAsync(job.TranscriptId.Value);
            if (transcript is null)
                return NotFound("Transcript not found.");

            job.MarkAsGenerating(job.TranscriptId.Value);
            await _jobRepo.UpdateAsync(job);
            await _notifier.NotifyAsync(id, "generating");

            var tonality = string.IsNullOrWhiteSpace(request.Tonality) ? "Auto" : request.Tonality;
            var platform = string.IsNullOrWhiteSpace(request.Platform) ? job.Platform : request.Platform;


            if (string.IsNullOrWhiteSpace(platform))
                return BadRequest("Platform is required.");

            var generation = await _generationService.GenerateAsync(
                userId,
                transcript.Id,
                transcript.Text,
                platform,
                tonality,
                cancellationToken);

            job.MarkAsDone(generation.Id);
            await _jobRepo.UpdateAsync(job);
            await _notifier.NotifyAsync(id, "done", new
            {
                generationId = generation.Id,
                title = generation.Title,
                hook = generation.Hook,
                hashtags = generation.Hashtags,
                tonality = generation.Tonality
            });

            return Ok(new GenerationResponseDto(
                generation.Id, generation.Platform, generation.Title,
                generation.Hook, generation.Hashtags, generation.Tonality,
                generation.ModelUsed, generation.PromptVersion,
                generation.RegenerationIndex, generation.CreatedAt));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var job = await _jobRepo.GetByIdAsync(id);
            if (job is null || job.UserId != userId)
                return NotFound();

            
            if (job.TranscriptId.HasValue)
                await _generationRepo.DeleteByTranscriptIdAsync(job.TranscriptId.Value);

            await _jobRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}