
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentHook.BL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ContentHook.BL.Services
{
    public class WhisperTranscriptionService : ITranscriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WhisperTranscriptionService> _logger;
        private readonly string _apiKey;

        public WhisperTranscriptionService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<WhisperTranscriptionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        }

        public async Task<string> TranscribeAsync(
            Stream audioStream,
            string language,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Calling Whisper API...");

            using var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(audioStream), "file", "audio.mp3");
            formData.Add(new StringContent("whisper-1"), "model");

            if (!string.IsNullOrWhiteSpace(language))
                formData.Add(new StringContent(language), "language");


            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/audio/transcriptions");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = formData;

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"Whisper API error {response.StatusCode}: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<WhisperResponse>(
                cancellationToken: cancellationToken);

            return result?.Text
                ?? throw new InvalidOperationException("Whisper API returned empty transcript.");
        }

        private record WhisperResponse(string Text);
    }
}