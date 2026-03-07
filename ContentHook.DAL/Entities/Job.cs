using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ContentHook.DAL.Entities
{
    public class Job
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public JobStatus Status { get; private set; }
        public string Platform { get; private set; } = string.Empty;

        public string OriginalFileName { get; private set; } = string.Empty; 
        public string VideoStorageKey { get; private set; } = string.Empty; 

        public Guid? TranscriptId { get; private set; }
        public Guid? GenerationId { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected Job() { }

        public Job(string userId, string platform, string originalFileName, string videoStorageKey)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));
            if (string.IsNullOrWhiteSpace(platform))
                throw new ArgumentException("Platform is required.", nameof(platform));
            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("OriginalFileName is required.", nameof(originalFileName));
            if (string.IsNullOrWhiteSpace(videoStorageKey))
                throw new ArgumentException("VideoStorageKey is required.", nameof(videoStorageKey));

            Id = Guid.NewGuid();
            UserId = userId.Trim();
            Platform = platform.Trim().ToLowerInvariant();
            OriginalFileName = originalFileName.Trim();
            VideoStorageKey = videoStorageKey.Trim();
            Status = JobStatus.Queued;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsTranscribing()
        {
            Status = JobStatus.Transcribing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsGenerating(Guid transcriptId)
        {
            TranscriptId = transcriptId;
            Status = JobStatus.Generating;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsTranscribed(Guid transcriptId)
        {
            TranscriptId = transcriptId;
            Status = JobStatus.Transcribed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDone(Guid generationId)
        {
            GenerationId = generationId;
            Status = JobStatus.Done;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Status = JobStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum JobStatus
    {
        Queued,
        Transcribing,
        Transcribed,
        Generating,
        Done,
        Failed
    }
}