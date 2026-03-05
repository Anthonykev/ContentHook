using ContentHook.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContentHook.BL.Interfaces
{
    public interface IPromptBuilder
{
    
    // Baut den System-Prompt mit plattformspezifischen Regeln.
    string BuildSystemPrompt(PlatformRules rules);

    
    // Baut den User-Prompt mit dem Transkript-Text.
    string BuildUserPrompt(string transcriptText);
}
}
