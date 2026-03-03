using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.DAL.Entities
{
    public class Transcript
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Language { get; set; }
        public string? OriginalFileName { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        protected Transcript() { }

        public Transcript(string userId, string text, string? language = null, string? originalFileName = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text is required.", nameof(text));

            if (text.Length > 20000)
                throw new ArgumentException("Text must not exceed 20,000 characters.", nameof(text));

            Id = Guid.NewGuid();
            UserId = userId.Trim();
            Text = text.Trim();
            Language = language?.Trim();
            OriginalFileName = originalFileName?.Trim();
            CreatedAt = DateTime.UtcNow;
        }
    }
}

