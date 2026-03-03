
using System.ComponentModel.DataAnnotations;

namespace ContentHook.API.DTOs
{
    public class CreateTranscriptRequest
    {
        [Required]
        [MinLength(10, ErrorMessage = "Text must be at least 10 characters.")]
        [MaxLength(20000, ErrorMessage = "Text must not exceed 20,000 characters.")]
        public string Text { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Language { get; set; }
    }
}
