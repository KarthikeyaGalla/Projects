using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Text_Embedding.Interface;
using Microsoft.Extensions.Logging;

namespace Text_Embedding.Models
{
    public class GeminiServices : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiServices> _logger;
        private readonly string _apiKey;

        public GeminiServices(HttpClient httpClient, ILogger<GeminiServices> logger, string apiKey)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = apiKey;
        }

        // Implementing GenerateContentAsync
        public async Task<string> GenerateContentAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to generate content for prompt: '{prompt}'. Error: {error}");
                    throw new HttpRequestException($"Error {response.StatusCode}: {error}");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var contentText = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    return contentText ?? "No response text found.";
                }

                throw new HttpRequestException("No valid candidates or content found in the response.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while generating content for prompt: '{prompt}'. Error: {ex.Message}");
                throw;
            }
        }

        // Implementing GenerateEmbeddingAsync
        public async Task<double[]> GenerateEmbeddingAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateEmbedding?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to generate embedding for prompt: '{prompt}'. Error: {error}");
                    throw new HttpRequestException($"Error {response.StatusCode}: {error}");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonString);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("embedding", out var embedding))
                {
                    // Assuming the embedding is returned as an array of doubles
                    var embeddingArray = embedding.EnumerateArray().Select(e => e.GetDouble()).ToArray();
                    return embeddingArray;
                }

                throw new HttpRequestException("No embedding found in the response.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while generating embedding for prompt: '{prompt}'. Error: {ex.Message}");
                throw;
            }
        }
    }
}
