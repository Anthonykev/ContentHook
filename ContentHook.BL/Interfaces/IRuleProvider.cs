using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentHook.BL.Interfaces
{
    public interface IRuleProvider
    {
      
        PlatformRules GetRules(string platform);
    }

    public record PlatformRules(
        string Platform,
        string PromptVersion,
        TitleRules Title,
        HookRules Hook,
        HashtagRules Hashtags
    );

    public record TitleRules(
        int MinChars,
        int MaxChars,
        string Tone,
        bool EmojiAllowed,
        string Description
    );

    public record HookRules(
        int MaxWords,
        string Tone,
        bool EmojiAllowed,
        string Description
    );

    public record HashtagRules(
        int MinCount,
        int MaxCount,
        string Structure,
        string Description
    );
}
