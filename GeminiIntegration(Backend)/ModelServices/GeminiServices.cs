using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GeminiIntegration.Interface;
using System.Text.RegularExpressions;


namespace GeminiIntegration.ModelServices
{

    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "AIzaSyDUDyWtp7Zb7wWj_d1xS4aPk3G1Gei1lYM";

        public GeminiService()
        {
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        }

        //public async Task<string> GenerateContentAsync(string prompt)
        //{
        //    var request = new
        //    {
        //        contents = new[]
        //        {
        //            new
        //            {
        //                parts = new[]
        //                {
        //                    new { text = prompt }
        //                }
        //            }
        //        }
        //    };

        //    var json = JsonSerializer.Serialize(request);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync(
        //        "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=" + ApiKey,
        //        content);

        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(responseString);
        //    var text = doc.RootElement
        //        .GetProperty("candidates")[0]
        //        .GetProperty("content")
        //        .GetProperty("parts")[0]
        //        .GetProperty("text")
        //        .GetString();

        //    return text;
        //}

        public async Task<string> GenerateContentAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={ApiKey}";

            // Safely construct the request body using anonymous objects
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

            // Serialize safely
            var json = JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode} - {error}";
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            // Parse and extract the generated content
            using var jsonDoc = JsonDocument.Parse(jsonString);
            var root = jsonDoc.RootElement;

            var text = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "No response text found.";
        }



        //return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
    }
}

