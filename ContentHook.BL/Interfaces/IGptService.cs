using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IGptService
    {
        Task<GptGenerationResult> GenerateAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default);
    }

    public record GptGenerationResult(
        string Title,
        string Hook,
        string Hashtags,
        string ModelUsed
    );
}