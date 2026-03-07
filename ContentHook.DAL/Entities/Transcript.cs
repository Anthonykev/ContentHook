using System;

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
        public DateTime UpdatedAt { get; private set; }

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
            UpdatedAt = DateTime.UtcNow;  
        }

        public void UpdateText(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("Text cannot be empty.", nameof(newText));
            if (newText.Length > 20000)
                throw new ArgumentException("Text must not exceed 20,000 characters.", nameof(newText));

            Text = newText.Trim();
            UpdatedAt = DateTime.UtcNow;  
        }
    }
}