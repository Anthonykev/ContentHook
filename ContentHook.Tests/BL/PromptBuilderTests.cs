using ContentHook.BL.Interfaces;
using ContentHook.BL.Services;
using FluentAssertions;

namespace ContentHook.Tests.BL
{
    public class PromptBuilderTests
    {
        // PromptBuilder hat keine Abhängigkeiten — kein Mock nötig
        private readonly PromptBuilder _sut = new();

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

        [Fact]
        public void BuildSystemPrompt_TikTok_ContainsPlatformName()
        {
            var result = _sut.BuildSystemPrompt(BuildTikTokRules());
            result.Should().Contain("TIKTOK");
        }

    

        [Fact]
        public void BuildSystemPrompt_Instagram_ContainsPlatformName()
        {
            var result = _sut.BuildSystemPrompt(BuildInstagramRules());
            result.Should().Contain("INSTAGRAM");
        }

      

        [Fact]
        public void BuildSystemPrompt_TikTokVsInstagram_DifferentTitleLimits()
        {
            var tiktokPrompt = _sut.BuildSystemPrompt(BuildTikTokRules());
            var instagramPrompt = _sut.BuildSystemPrompt(BuildInstagramRules());

            // TikTok max 80, Instagram max 100
            tiktokPrompt.Should().Contain("80");
            instagramPrompt.Should().Contain("100");
        }


        [Fact]
        public void BuildSystemPrompt_TikTokVsInstagram_DifferentHashtagLimits()
        {
            var tiktokPrompt = _sut.BuildSystemPrompt(BuildTikTokRules());
            var instagramPrompt = _sut.BuildSystemPrompt(BuildInstagramRules());

            // TikTok max 8, Instagram max 15
            tiktokPrompt.Should().Contain("8");
            instagramPrompt.Should().Contain("15");
        }

    

        [Fact]
        public void BuildSystemPrompt_AutoTonality_ContainsAutoInstruction()
        {
            var result = _sut.BuildSystemPrompt(BuildTikTokRules(), tonality: "Auto");

            result.Should().Contain("Wähle selbst die passende Tonalität");
        }



        [Theory]
        [InlineData("Emotional")]
        [InlineData("Sachlich")]
        [InlineData("Neugierig")]
        [InlineData("Humorvoll")]
        [InlineData("Sarkastisch")]
        [InlineData("Inspirierend")]
        [InlineData("Provokant")]
        public void BuildSystemPrompt_CustomTonality_ContainsCustomValue(string tonality)
        {
            var result = _sut.BuildSystemPrompt(BuildTikTokRules(), tonality: tonality);

            // Tonalität muss im Prompt stehen 
            result.Should().Contain(tonality);
            // Auto-Instruktion darf NICHT drin sein
            result.Should().NotContain("Wähle selbst die passende Tonalität");
        }

       

        [Fact]
        public void BuildSystemPrompt_AlwaysContainsJsonOutputInstruction()
        {
            var result = _sut.BuildSystemPrompt(BuildTikTokRules());

            result.Should().Contain("title");
            result.Should().Contain("hook");
            result.Should().Contain("hashtags");
            result.Should().Contain("JSON");
        }

      

        [Fact]
        public void BuildUserPrompt_ContainsTranscriptText()
        {
            var transcript = "Das ist mein Test-Transkript über Software Development.";
            var result = _sut.BuildUserPrompt(transcript);
            result.Should().Contain(transcript);
        }



        [Fact]
        public void BuildUserPrompt_LongTranscript_IsTruncatedTo3000Chars()
        {
            var longTranscript = new string('a', 5000);
            var result = _sut.BuildUserPrompt(longTranscript);

            result.Should().Contain("...");
            result.Length.Should().BeLessThan(4000);
        }

   

        [Fact]
        public void BuildUserPrompt_ShortTranscript_IsNotTruncated()
        {
            var shortTranscript = "Kurzes Video über React Hooks.";
            var result = _sut.BuildUserPrompt(shortTranscript);

            result.Should().NotContain("...");
            result.Should().Contain(shortTranscript);
        }
    }
}