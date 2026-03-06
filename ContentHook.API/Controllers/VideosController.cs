using ContentHook.BL.Interfaces;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ContentHook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideosController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly IJobQueue _jobQueue;
        private readonly IVideoStorageService _storage;

        public VideosController(
            IJobRepository jobRepository,
            IJobQueue jobQueue,
            IVideoStorageService storage)
        {
            _jobRepository = jobRepository;
            _jobQueue = jobQueue;
            _storage = storage;
        }

        [HttpPost]
        [RequestSizeLimit(500 * 1024 * 1024)] // 500 MB Limit
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromQuery] string platform,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(platform) ||
                (platform != "tiktok" && platform != "instagram"))
                return BadRequest("Platform must be 'tiktok' or 'instagram'.");

            if (file is null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedTypes = new[] { "video/mp4", "video/quicktime" };
            if (!allowedTypes.Contains(file.ContentType.ToLowerInvariant()))
                return BadRequest($"Invalid file type '{file.ContentType}'. Only MP4 and MOV are allowed.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("UserId not found in token.");


            await using var stream = file.OpenReadStream();
            var storageKey = await _storage.SaveAsync(
                stream, file.FileName, cancellationToken);

            var job = new Job(userId, platform, file.FileName, storageKey);
            await _jobRepository.AddAsync(job);
            await _jobQueue.EnqueueAsync(job.Id, cancellationToken);

            return AcceptedAtAction(
                nameof(GetJobStatus),
                new { id = job.Id },
                new { jobId = job.Id, status = job.Status.ToString() }
            );
        }

        [HttpGet("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJobStatus(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job is null) return NotFound();

            return Ok(new
            {
                jobId = job.Id,
                status = job.Status.ToString(),
                platform = job.Platform,
                transcriptId = job.TranscriptId,
                generationId = job.GenerationId,
                errorMessage = job.ErrorMessage,
                createdAt = job.CreatedAt,
                updatedAt = job.UpdatedAt
            });
        }
    }
}