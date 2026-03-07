namespace ContentHook.API.DTOs
{
    public record HistoryItemDto(
        Guid JobId,
        string OriginalFileName,
        string Platform,
        string Status,
        Guid? TranscriptId,
        string? TranscriptText,
        DateTime CreatedAt,
        List<GenerationResponseDto> Generations
    );
}