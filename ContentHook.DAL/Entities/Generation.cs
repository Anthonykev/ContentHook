using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.DAL.Entities
{
    public class Generation
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public Guid TranscriptId { get; private set; }
        public string Platform { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string Hook { get; private set; } = string.Empty;
        public string Hashtags { get; private set; } = string.Empty;

        
        public string ModelUsed { get; private set; } = string.Empty;
        public string PromptVersion { get; private set; } = string.Empty;
        public string Tonality { get; private set; } = "Standard";


        //  Zählt max 3 pro Transcript pro Platform
        public int RegenerationIndex { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Generation() { }

        public Generation(
            string userId,
            Guid transcriptId,
            string platform,
            string title,
            string hook,
            string hashtags,
            string modelUsed,
            string promptVersion,
            int regenerationIndex,
            string tonality = "Standard")
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));
            if (string.IsNullOrWhiteSpace(hook))
                throw new ArgumentException("Hook is required.", nameof(hook));
            if (string.IsNullOrWhiteSpace(hashtags))
                throw new ArgumentException("Hashtags is required.", nameof(hashtags));

            Id = Guid.NewGuid();
            UserId = userId.Trim();
            TranscriptId = transcriptId;
            Platform = platform.Trim().ToLowerInvariant();
            Title = title.Trim();
            Hook = hook.Trim();
            Hashtags = hashtags.Trim();
            ModelUsed = modelUsed.Trim();
            PromptVersion = promptVersion.Trim();
            RegenerationIndex = regenerationIndex;
            Tonality = tonality.Trim();
            CreatedAt = DateTime.UtcNow;
        }
    }
}