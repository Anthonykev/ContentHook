using ContentHook.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ContentHook.BL.Services
{
    public class RuleProvider : IRuleProvider
    {
        private readonly ILogger<RuleProvider> _logger;
        private readonly string _rulesDirectory;

       
        private readonly ConcurrentDictionary<string, PlatformRules> _cache = new();

        public RuleProvider(ILogger<RuleProvider> logger)
        {
            _logger = logger;
            _rulesDirectory = Path.Combine(AppContext.BaseDirectory, "rules");
        }

        public PlatformRules GetRules(string platform)
        {
            platform = platform.ToLowerInvariant().Trim();

            return _cache.GetOrAdd(platform, p =>
            {
                var filePath = Path.Combine(_rulesDirectory, $"{p}.json");

                if (!File.Exists(filePath))
                    throw new InvalidOperationException(
                        $"No rules found for platform '{p}'. Expected: {filePath}");

                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var raw = JsonSerializer.Deserialize<RawPlatformRules>(json, options)
                    ?? throw new InvalidOperationException($"Could not parse rules for '{p}'.");

                var rules = new PlatformRules(
                    raw.Platform,
                    raw.PromptVersion,
                    new TitleRules(raw.Rules.Title.MinChars, raw.Rules.Title.MaxChars,
                        raw.Rules.Title.Tone, raw.Rules.Title.EmojiAllowed, raw.Rules.Title.Description),
                    new HookRules(raw.Rules.Hook.MaxWords, raw.Rules.Hook.Tone,
                        raw.Rules.Hook.EmojiAllowed, raw.Rules.Hook.Description),
                    new HashtagRules(raw.Rules.Hashtags.MinCount, raw.Rules.Hashtags.MaxCount,
                        raw.Rules.Hashtags.Structure, raw.Rules.Hashtags.Description)
                );

                _logger.LogInformation("Rules loaded for platform: {Platform} (v{Version})",
                    p, rules.PromptVersion);

                return rules;
            });
        }

      
        private record RawPlatformRules(string Platform, string PromptVersion, RawRules Rules);
        private record RawRules(RawTitleRules Title, RawHookRules Hook, RawHashtagRules Hashtags);
        private record RawTitleRules(int MinChars, int MaxChars, string Tone, bool EmojiAllowed, string Description);
        private record RawHookRules(int MaxWords, string Tone, bool EmojiAllowed, string Description);
        private record RawHashtagRules(int MinCount, int MaxCount, string Structure, string Description);
    }
}