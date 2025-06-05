using Amazon.Runtime;
using Amazon;
using SpeechBot.Models;

namespace SpeechBot.Services
{
    public class NovaService
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _region;

        public NovaService(IConfiguration config)
        {
            _accessKey = config["AWS:AccessKey"];
            _secretKey = config["AWS:SecretKey"];
            _region = config["AWS:Region"];
        }

        public async Task<NovaResponse> SendToNovaAsync(string prompt, IFormFile audioInput)
        {
            // TODO: Replace this block with actual AWS Nova Sonic call
            var dummyAudio = await File.ReadAllBytesAsync("wwwroot/dummy-response.wav");

            return new NovaResponse
            {
                Transcript = $"Mocked response for prompt: {prompt}",
                AudioData = dummyAudio
            };
        }
    }
}
