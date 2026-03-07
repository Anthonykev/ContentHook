using System.ComponentModel.DataAnnotations;

namespace ContentHook.API.DTOs
{
    public class UpdateTranscriptRequest
    {
        [Required]
        [MinLength(10)]
        [MaxLength(20000)]
        public string Text { get; set; } = string.Empty;
    }
}