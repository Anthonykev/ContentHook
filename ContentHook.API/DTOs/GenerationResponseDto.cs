namespace ContentHook.API.DTOs
{
    public record GenerationResponseDto(
        Guid Id,
        string Platform,
        string Title,
        string Hook,
        string Hashtags,
        string ModelUsed,
        string PromptVersion,
        int RegenerationIndex,
        DateTime CreatedAt
    );
}