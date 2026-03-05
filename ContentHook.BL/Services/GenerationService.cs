using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.BL.Interfaces;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContentHook.BL.Services
{
    public class GenerationService : IGenerationService
    {
        private readonly IGptService _gptService;
        private readonly IRuleProvider _ruleProvider;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IGenerationRepository _generationRepo;
        private readonly ILogger<GenerationService> _logger;

        private const int MaxGenerationsPerTranscriptPerPlatform = 3;

        public GenerationService(
            IGptService gptService,
            IRuleProvider ruleProvider,
            IPromptBuilder promptBuilder,
            IGenerationRepository generationRepo,
            ILogger<GenerationService> logger)
        {
            _gptService = gptService;
            _ruleProvider = ruleProvider;
            _promptBuilder = promptBuilder;
            _generationRepo = generationRepo;
            _logger = logger;
        }

        public async Task<Generation> GenerateAsync(
            string userId,
            Guid transcriptId,
            string transcriptText,
            string platform,
            CancellationToken cancellationToken = default)
        {
            //  Max-3 pro Transcript pro Platform 
            var existingCount = await _generationRepo
                .CountByTranscriptAndPlatformAsync(transcriptId, platform);

            if (existingCount >= MaxGenerationsPerTranscriptPerPlatform)
                throw new InvalidOperationException(
                    $"Maximum of {MaxGenerationsPerTranscriptPerPlatform} generations " +
                    $"per transcript per platform reached. " +
                    $"(TranscriptId: {transcriptId}, Platform: {platform})");

            // Regeln laden + Prompt bauen
            var rules = _ruleProvider.GetRules(platform);
            var systemPrompt = _promptBuilder.BuildSystemPrompt(rules);
            var userPrompt = _promptBuilder.BuildUserPrompt(transcriptText);

            _logger.LogInformation(
                "Generating for platform {Platform}, prompt version {Version}, attempt {Index}",
                platform, rules.PromptVersion, existingCount + 1);

            // GPT aufrufen
            var result = await _gptService.GenerateAsync(
                systemPrompt, userPrompt, cancellationToken);

            // Generation in DB speichern 
            var generation = new Generation(
                userId,
                transcriptId,
                platform,
                result.Title,
                result.Hook,
                result.Hashtags,
                result.ModelUsed,
                rules.PromptVersion,
                regenerationIndex: existingCount + 1);

            await _generationRepo.AddAsync(generation);

            _logger.LogInformation(
                "Generation saved. Id: {Id}, Platform: {Platform}, Index: {Index}",
                generation.Id, generation.Platform, generation.RegenerationIndex);

            return generation;
        }
    }
}