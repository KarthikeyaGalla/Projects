using SpeechBot.Models;
using System.Text;
using System.Text.Json;
using NAudio.Wave;


public class NovaSpeechService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public NovaSpeechService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<NovaResponse> SendToNovaAsync(string prompt, IFormFile audioInput)
    {
        try
        {
            using var mp3Stream = audioInput.OpenReadStream();
            using var mp3Reader = new Mp3FileReader(mp3Stream);
            using var pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
            using var ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, pcmStream);
            var pcmBytes = ms.ToArray();
            var base64Audio = Convert.ToBase64String(pcmBytes);

            var payload = new
            {
                @event = new
                {
                    sessionStart = new
                    {
                        inferenceConfiguration = new
                        {
                            maxTokens = 1024,
                            topP = 0.9,
                            temperature = 0.7
                        }
                    },
                    promptStart = new
                    {
                        promptName = "speech",
                        textOutputConfiguration = new { mediaType = "text/plain" },
                        audioOutputConfiguration = new
                        {
                            mediaType = "audio/lpcm",
                            sampleRateHertz = 24000,
                            sampleSizeBits = 16,
                            channelCount = 1,
                            voiceId = "matthew",
                            encoding = "base64",
                            audioType = "SPEECH"
                        }
                    },
                    contentStart = new
                    {
                        promptName = "speech",
                        contentName = "content",
                        type = "TEXT",
                        role = "SYSTEM",
                        interactive = true,
                        textInputConfiguration = new { mediaType = "text/plain" }
                    },
                    textInput = new
                    {
                        promptName = "speech",
                        contentName = "content",
                        content = prompt
                    },
                    audioInput = new
                    {
                        promptName = "speech",
                        contentName = "content",
                        type = "AUDIO",
                        mediaType = "audio/lpcm",
                        sampleRateHertz = 24000,
                        sampleSizeBits = 16,
                        channelCount = 1,
                        encoding = "base64",
                        content = base64Audio
                    },
                    contentEnd = new { promptName = "speech", contentName = "content" },
                    promptEnd = new { promptName = "speech" },
                    sessionEnd = new { }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var apiUrl = _config["NovaApiUrl"];
            var response = await _httpClient.PostAsync(apiUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resultJson);
            var root = doc.RootElement;
            var results = root.GetProperty("results")[0];

            string transcript = results.GetProperty("text").GetString();
            string audioBase64 = results.GetProperty("audio").GetString();
            byte[] audioBytes = Convert.FromBase64String(audioBase64);

            return new NovaResponse { Transcript = transcript, AudioData = audioBytes };
        }
        catch (Exception ex)
        {
            return new NovaResponse { Transcript = $"Error: {ex.Message}", AudioData = null };
        }
    }
}