using ContentHook.BL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ContentHook.BL.Services
{
    public class GptService : IGptService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GptService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

      
        private static readonly object ResponseSchema = new
        {
            type = "object",
            properties = new
            {
                title = new { type = "string" },
                hook = new { type = "string" },
                hashtags = new { type = "string" }
            },
            required = new[] { "title", "hook", "hashtags" },
            additionalProperties = false
        };

        public GptService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GptService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");

            _model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";
        }

        public async Task<GptGenerationResult> GenerateAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Calling GPT model: {Model}", _model);

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = userPrompt   }
                },
                temperature = 0.7,
                max_tokens = 300,
                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "content_generation",
                        strict = true,
                        schema = ResponseSchema
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"GPT API error {response.StatusCode}: {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return ParseGptResponse(responseJson);
        }

        private GptGenerationResult ParseGptResponse(string responseJson)
        {
            string? rawContent = null;

            try
            {
                using var doc = JsonDocument.Parse(responseJson);

                // Tatsächlich verwendetes Modell aus der API-Antwort lesen
                var actualModel = doc.RootElement
                    .GetProperty("model")
                    .GetString() ?? _model;

                rawContent = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()
                    ?? throw new InvalidOperationException("GPT returned empty content.");

                using var contentDoc = JsonDocument.Parse(rawContent);
                var root = contentDoc.RootElement;

                var title = root.GetProperty("title").GetString()
                    ?? throw new InvalidOperationException("Missing 'title' in GPT response.");
                var hook = root.GetProperty("hook").GetString()
                    ?? throw new InvalidOperationException("Missing 'hook' in GPT response.");
                var hashtags = root.GetProperty("hashtags").GetString()
                    ?? throw new InvalidOperationException("Missing 'hashtags' in GPT response.");

                _logger.LogInformation("GPT response parsed. Title: {Title}", title);
                return new GptGenerationResult(title, hook, hashtags, actualModel);
            }
            catch (JsonException ex)
            {
                // Raw content loggen für Debugging
                _logger.LogError(ex, "Failed to parse GPT response. Raw content: {Raw}", rawContent);
                throw new InvalidOperationException(
                    $"GPT response parsing failed. Raw: {rawContent}", ex);
            }
        }
    }
}