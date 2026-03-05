using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContentHook.BL.Interfaces;

namespace ContentHook.BL.Services
{
    public class PromptBuilder : IPromptBuilder
    {
        public string BuildSystemPrompt(PlatformRules rules)
        {
            return "Du bist ein Social-Media-Experte für " + rules.Platform.ToUpper() + ".\n\n" +
                   "Deine Aufgabe ist es, aus einem Video-Transkript folgende Elemente zu generieren:\n\n" +
                   "TITEL-REGELN:\n" +
                   $"- Länge: {rules.Title.MinChars}–{rules.Title.MaxChars} Zeichen\n" +
                   $"- Tonalität: {rules.Title.Tone}\n" +
                   $"- Emoji erlaubt: {(rules.Title.EmojiAllowed ? "Ja" : "Nein")}\n\n" +
                   "HOOK-REGELN (wird in den ersten 3 Sekunden eingeblendet):\n" +
                   $"- Maximale Wörter: {rules.Hook.MaxWords}\n" +
                   $"- Tonalität: {rules.Hook.Tone}\n" +
                   $"- Emoji erlaubt: {(rules.Hook.EmojiAllowed ? "Ja" : "Nein")}\n" +
                   "- WICHTIG: Der Hook ist KEIN Satz — nur 2-5 prägnante Wörter\n\n" +
                   "HASHTAG-REGELN:\n" +
                   $"- Anzahl: {rules.Hashtags.MinCount}–{rules.Hashtags.MaxCount} Hashtags\n" +
                   $"- Struktur: {rules.Hashtags.Structure}\n\n" +
                   "AUSGABE-FORMAT (nur JSON, kein Text davor oder danach):\n" +
                   "{\n" +
                   "  \"title\": \"...\",\n" +
                   "  \"hook\": \"...\",\n" +
                   "  \"hashtags\": \"#tag1 #tag2 #tag3\"\n" +
                   "}\n\n" +
                   "Antworte AUSSCHLIESSLICH mit dem JSON-Objekt. Kein erklärender Text.";
        }

        public string BuildUserPrompt(string transcriptText)
        {
            var truncated = transcriptText.Length > 3000
                ? transcriptText[..3000] + "..."
                : transcriptText;

            return "Transkript:\n" + truncated + "\n\nGeneriere jetzt Titel, Hook und Hashtags für dieses Video.";
        }
    }
}