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
        // Auth0 "sub" 
        public string UserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Language { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        protected Transcript() { } 

     
        public Transcript(string userId, string text, string? language = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text is required");

            Id = Guid.NewGuid();
            UserId = userId.Trim();
            Text = text.Trim();
            Language = language;
            CreatedAt = DateTime.UtcNow;
        }

    }
}

