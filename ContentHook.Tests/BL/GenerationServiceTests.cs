using ContentHook.BL.Interfaces;
using ContentHook.BL.Services;
using ContentHook.DAL.Entities;
using ContentHook.DAL.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ContentHook.Tests.BL
{
    public class GenerationServiceTests
    {
        // ────────────────────────────────────────────────
        // Shared test helpers
        // ────────────────────────────────────────────────

        private static PlatformRules BuildTikTokRules() => new(
            Platform: "tiktok",
            PromptVersion: "v1.0",
            Title: new TitleRules(MinChars: 20, MaxChars: 80, Tone: "energetic", EmojiAllowed: true, Description: ""),
            Hook: new HookRules(MaxWords: 5, Tone: "bold", EmojiAllowed: true, Description: ""),
            Hashtags: new HashtagRules(MinCount: 3, MaxCount: 8, Structure: "niche+trend", Description: "")
        );

        private static PlatformRules BuildInstagramRules() => new(
            Platform: "instagram",
            PromptVersion: "v1.0",
            Title: new TitleRules(MinChars: 15, MaxChars: 100, Tone: "aesthetic", EmojiAllowed: true, Description: ""),
            Hook: new HookRules(MaxWords: 6, Tone: "calm", EmojiAllowed: false, Description: ""),
            Hashtags: new HashtagRules(MinCount: 5, MaxCount: 15, Structure: "mix", Description: "")
        );

        private static (
            Mock<IGptService> gpt,
            Mock<IRuleProvider> rules,
            Mock<IPromptBuilder> prompt,
            Mock<IGenerationRepository> repo,
            GenerationService sut
        ) BuildSut()
        {
            var gpt = new Mock<IGptService>();
            var rules = new Mock<IRuleProvider>();
            var prompt = new Mock<IPromptBuilder>();
            var repo = new Mock<IGenerationRepository>();
            var logger = NullLogger<GenerationService>.Instance;

            var sut = new GenerationService(gpt.Object, rules.Object, prompt.Object, repo.Object, logger);
            return (gpt, rules, prompt, repo, sut);
        }

        

        [Fact]
        public async Task GenerateAsync_TikTok_HappyPath_ReturnsGeneration()
        {
            
            var (gpt, rulesMock, promptMock, repo, sut) = BuildSut();

            var transcriptId = Guid.NewGuid();
            var userId = "auth0|user123";

            rulesMock.Setup(r => r.GetRules("tiktok")).Returns(BuildTikTokRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), It.IsAny<string>()))
                      .Returns("system-prompt");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user-prompt");

            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok"))
                .ReturnsAsync(0);

            gpt.Setup(g => g.GenerateAsync("system-prompt", "user-prompt", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GptGenerationResult(
                   Title: "So läuft Softwareentwicklung 2025",
                   Hook: "Krass oder?",
                   Hashtags: "#coding #dev #tech",
                   ModelUsed: "gpt-4o-mini"));

            repo.Setup(r => r.AddAsync(It.IsAny<Generation>()))
                .ReturnsAsync((Generation g) => g);

         
            var result = await sut.GenerateAsync(userId, transcriptId, "Transcript text here", "tiktok");

           
            result.Should().NotBeNull();
            result.Title.Should().Be("So läuft Softwareentwicklung 2025");
            result.Hook.Should().Be("Krass oder?");
            result.Hashtags.Should().Be("#coding #dev #tech");
            result.Platform.Should().Be("tiktok");
            result.RegenerationIndex.Should().Be(1);
            result.Tonality.Should().Be("Auto");

            repo.Verify(r => r.AddAsync(It.IsAny<Generation>()), Times.Once);
        }

      

        [Fact]
        public async Task GenerateAsync_Instagram_HappyPath_ReturnsGeneration()
        {
           
            var (gpt, rulesMock, promptMock, repo, sut) = BuildSut();

            var transcriptId = Guid.NewGuid();
            var userId = "auth0|user456";

            rulesMock.Setup(r => r.GetRules("instagram")).Returns(BuildInstagramRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), It.IsAny<string>()))
                      .Returns("system-prompt-ig");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user-prompt-ig");

            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "instagram"))
                .ReturnsAsync(0);

            gpt.Setup(g => g.GenerateAsync("system-prompt-ig", "user-prompt-ig", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GptGenerationResult(
                   Title: "Meine Content Routine ✨",
                   Hook: "Das musst du wissen",
                   Hashtags: "#content #lifestyle #ig",
                   ModelUsed: "gpt-4o-mini"));

            repo.Setup(r => r.AddAsync(It.IsAny<Generation>()))
                .ReturnsAsync((Generation g) => g);

        
            var result = await sut.GenerateAsync(userId, transcriptId, "Transcript text here", "instagram");

           
            result.Platform.Should().Be("instagram");
            result.Title.Should().Be("Meine Content Routine ✨");
        }

        

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(10)]
        public async Task GenerateAsync_WhenMaxGenerationsReached_ThrowsInvalidOperationException(int existingCount)
        {
            
            var (_, rulesMock, promptMock, repo, sut) = BuildSut();

            var transcriptId = Guid.NewGuid();

            rulesMock.Setup(r => r.GetRules("tiktok")).Returns(BuildTikTokRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), It.IsAny<string>()))
                      .Returns("system");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user");

            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok"))
                .ReturnsAsync(existingCount);

          
            var act = () => sut.GenerateAsync("user1", transcriptId, "text", "tiktok");

            
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Maximum of 3 generations*");

            repo.Verify(r => r.AddAsync(It.IsAny<Generation>()), Times.Never);
        }

     

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GenerateAsync_WhenUnderMaxGenerations_Succeeds(int existingCount)
        {
            var (gpt, rulesMock, promptMock, repo, sut) = BuildSut();
            var transcriptId = Guid.NewGuid();

            rulesMock.Setup(r => r.GetRules("tiktok")).Returns(BuildTikTokRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), It.IsAny<string>()))
                      .Returns("system");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user");
            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok"))
                .ReturnsAsync(existingCount);
            gpt.Setup(g => g.GenerateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GptGenerationResult("Title", "Hook", "#tag", "gpt-4o-mini"));
            repo.Setup(r => r.AddAsync(It.IsAny<Generation>()))
                .ReturnsAsync((Generation g) => g);

            var result = await sut.GenerateAsync("user1", transcriptId, "text", "tiktok");

            result.Should().NotBeNull();
            result.RegenerationIndex.Should().Be(existingCount + 1);
        }


        [Fact]
        public async Task GenerateAsync_CustomTonality_IsPassedToPromptBuilder()
        {
         
            var (gpt, rulesMock, promptMock, repo, sut) = BuildSut();
            var transcriptId = Guid.NewGuid();

            rulesMock.Setup(r => r.GetRules("tiktok")).Returns(BuildTikTokRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), "Humorvoll"))
                      .Returns("system-humorvoll");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user");
            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok"))
                .ReturnsAsync(0);
            gpt.Setup(g => g.GenerateAsync("system-humorvoll", "user", It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GptGenerationResult("Title", "Hook", "#tag", "gpt-4o-mini"));
            repo.Setup(r => r.AddAsync(It.IsAny<Generation>()))
                .ReturnsAsync((Generation g) => g);

        
            var result = await sut.GenerateAsync("user1", transcriptId, "text", "tiktok", tonality: "Humorvoll");

          
            result.Tonality.Should().Be("Humorvoll");
            promptMock.Verify(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), "Humorvoll"), Times.Once);
        }

      

        [Fact]
        public async Task GenerateAsync_DefaultTonality_IsAuto()
        {
            
            var (gpt, rulesMock, promptMock, repo, sut) = BuildSut();
            var transcriptId = Guid.NewGuid();

            rulesMock.Setup(r => r.GetRules("tiktok")).Returns(BuildTikTokRules());
            promptMock.Setup(p => p.BuildSystemPrompt(It.IsAny<PlatformRules>(), "Auto"))
                      .Returns("system");
            promptMock.Setup(p => p.BuildUserPrompt(It.IsAny<string>()))
                      .Returns("user");
            repo.Setup(r => r.CountByTranscriptAndPlatformAsync(transcriptId, "tiktok"))
                .ReturnsAsync(0);
            gpt.Setup(g => g.GenerateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GptGenerationResult("Title", "Hook", "#tag", "gpt-4o-mini"));
            repo.Setup(r => r.AddAsync(It.IsAny<Generation>()))
                .ReturnsAsync((Generation g) => g);

           
            var result = await sut.GenerateAsync("user1", transcriptId, "text", "tiktok");

            result.Tonality.Should().Be("Auto");
        }
    }
}