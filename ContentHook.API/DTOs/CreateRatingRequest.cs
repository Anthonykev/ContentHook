using System.ComponentModel.DataAnnotations;

namespace ContentHook.API.DTOs
{
    public class CreateRatingRequest
    {
        // Background
        [Required] public string VideosPerMonth { get; set; } = string.Empty;
        [Required] public string MainPlatform { get; set; } = string.Empty;
        [Required] public string Experience { get; set; } = string.Empty;

        // Quality
        [Range(1, 5)] public int TitleScore { get; set; }
        [Range(1, 5)] public int HookScore { get; set; }
        [Range(1, 5)] public int HashtagScore { get; set; }
        [Range(1, 5)] public int PracticalScore { get; set; }
        [Range(1, 5)] public int OverallQualityScore { get; set; }

        // Platform
        [Range(1, 5)] public int PlatformFitScore { get; set; }
        [Range(1, 5)] public int PlatformInfluenceScore { get; set; }
        [Range(1, 5)] public int HashtagPlatformScore { get; set; }
        [Range(1, 5)] public int PlatformOptimizationScore { get; set; }

        // Usability
        [Range(1, 5)] public int UsabilityScore { get; set; }
        [Range(1, 5)] public int LayoutScore { get; set; }
        [Range(1, 5)] public int PresentationScore { get; set; }

        // Time Efficiency
        [Range(1, 5)] public int TimeSavingScore { get; set; }
        [Range(1, 5)] public int GenerationSpeedScore { get; set; }
        [Range(1, 5)] public int SupportScore { get; set; }
        [Range(1, 5)] public int EfficiencyComparisonScore { get; set; }

        // Overall
        [Range(1, 5)] public int OverallSatisfactionScore { get; set; }
        [Range(1, 5)] public int ReusabilityScore { get; set; }
        [Range(1, 5)] public int RecommendationScore { get; set; }

        [MaxLength(1000)] public string? Comment { get; set; }
    }
}