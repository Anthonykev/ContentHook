using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.DAL.Entities
{
    public class Evaluation
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; } = string.Empty;

        // Background
        public string VideosPerMonth { get; private set; } = string.Empty;
        public string MainPlatform { get; private set; } = string.Empty;
        public string Experience { get; private set; } = string.Empty;

        //  Quality
        public int TitleScore { get; private set; }
        public int HookScore { get; private set; }
        public int HashtagScore { get; private set; }
        public int PracticalScore { get; private set; }
        public int OverallQualityScore { get; private set; }

        // Platform Optimization
        public int PlatformFitScore { get; private set; }
        public int PlatformInfluenceScore { get; private set; }
        public int HashtagPlatformScore { get; private set; }
        public int PlatformOptimizationScore { get; private set; }

        // Usability
        public int UsabilityScore { get; private set; }
        public int LayoutScore { get; private set; }
        public int PresentationScore { get; private set; }

        // Time Efficiency & Support
        public int TimeSavingScore { get; private set; }
        public int GenerationSpeedScore { get; private set; }
        public int SupportScore { get; private set; }
        public int EfficiencyComparisonScore { get; private set; }

        // Overall Rating
        public int OverallSatisfactionScore { get; private set; }
        public int ReusabilityScore { get; private set; }
        public int RecommendationScore { get; private set; }

        // Free Text
        public string? Comment { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Evaluation() { }

        public Evaluation(
            string userId,
            string videosPerMonth, string mainPlatform, string experience,
            int titleScore, int hookScore, int hashtagScore, int practicalScore, int overallQualityScore,
            int platformFitScore, int platformInfluenceScore, int hashtagPlatformScore, int platformOptimizationScore,
            int usabilityScore, int layoutScore, int presentationScore,
            int timeSavingScore, int generationSpeedScore, int supportScore, int efficiencyComparisonScore,
            int overallSatisfactionScore, int reusabilityScore, int recommendationScore,
            string? comment)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            ValidateScore(titleScore, nameof(titleScore));
            ValidateScore(hookScore, nameof(hookScore));
            ValidateScore(hashtagScore, nameof(hashtagScore));
            ValidateScore(practicalScore, nameof(practicalScore));
            ValidateScore(overallQualityScore, nameof(overallQualityScore));
            ValidateScore(platformFitScore, nameof(platformFitScore));
            ValidateScore(platformInfluenceScore, nameof(platformInfluenceScore));
            ValidateScore(hashtagPlatformScore, nameof(hashtagPlatformScore));
            ValidateScore(platformOptimizationScore, nameof(platformOptimizationScore));
            ValidateScore(usabilityScore, nameof(usabilityScore));
            ValidateScore(layoutScore, nameof(layoutScore));
            ValidateScore(presentationScore, nameof(presentationScore));
            ValidateScore(timeSavingScore, nameof(timeSavingScore));
            ValidateScore(generationSpeedScore, nameof(generationSpeedScore));
            ValidateScore(supportScore, nameof(supportScore));
            ValidateScore(efficiencyComparisonScore, nameof(efficiencyComparisonScore));
            ValidateScore(overallSatisfactionScore, nameof(overallSatisfactionScore));
            ValidateScore(reusabilityScore, nameof(reusabilityScore));
            ValidateScore(recommendationScore, nameof(recommendationScore));

            Id = Guid.NewGuid();
            UserId = userId.Trim();
            VideosPerMonth = videosPerMonth.Trim();
            MainPlatform = mainPlatform.Trim();
            Experience = experience.Trim();
            TitleScore = titleScore; HookScore = hookScore; HashtagScore = hashtagScore;
            PracticalScore = practicalScore; OverallQualityScore = overallQualityScore;
            PlatformFitScore = platformFitScore; PlatformInfluenceScore = platformInfluenceScore;
            HashtagPlatformScore = hashtagPlatformScore; PlatformOptimizationScore = platformOptimizationScore;
            UsabilityScore = usabilityScore; LayoutScore = layoutScore; PresentationScore = presentationScore;
            TimeSavingScore = timeSavingScore; GenerationSpeedScore = generationSpeedScore;
            SupportScore = supportScore; EfficiencyComparisonScore = efficiencyComparisonScore;
            OverallSatisfactionScore = overallSatisfactionScore; ReusabilityScore = reusabilityScore;
            RecommendationScore = recommendationScore;
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            CreatedAt = DateTime.UtcNow;
        }

        private static void ValidateScore(int score, string name)
        {
            if (score < 1 || score > 5)
                throw new ArgumentOutOfRangeException(name, "Score must be between 1 and 5.");
        }
    }
}