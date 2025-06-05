using Microsoft.AspNetCore.Http;

namespace SpeechBot.Models
{
    public class NovaRequest
    {
        public string Prompt { get; set; }
        public IFormFile AudioInput { get; set; } // Optional
    }
}
