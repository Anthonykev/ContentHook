namespace ContentHook.API.DTOs
{
    public record RatingSummaryDto(
        int TotalRatings,
        double QualityAverage,
        double PlatformAverage,
        double UsabilityAverage,
        double TimeEfficiencyAverage,
        double OverallAverage
    );
}