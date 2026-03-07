namespace ContentHook.API.DTOs
{
    public class StartGenerationRequest
    {
        /// "Auto", "Emotional", "Sachlich", "Neugierig"
        public string Tonality { get; set; } = "Auto";
    }
}