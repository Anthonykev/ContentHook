namespace ContentHook.API.DTOs
{
    public class StartGenerationRequest
    {
        public string Tonality { get; set; } = "Auto";
        public string? Platform { get; set; }
    }
}