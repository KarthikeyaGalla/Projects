using Microsoft.AspNetCore.Mvc;
using SpeechBot.Models;
using SpeechBot.Services;

namespace SpeechBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private readonly NovaSpeechService _novaService;
        private readonly AzureStorageService _azureService;

        public TranscriptController(NovaSpeechService novaService, AzureStorageService azureService)
        {
            _novaService = novaService;
            _azureService = azureService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessRequest([FromForm] NovaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is required.");

            if (request.AudioInput == null || request.AudioInput.Length == 0)
                return BadRequest("Audio input is required.");

            var result = await _novaService.SendToNovaAsync(request.Prompt, request.AudioInput);
            await _azureService.SaveTranscriptAsync(request.Prompt, result.Transcript);

            if (result.AudioData == null)
                return Ok(new { Transcript = result.Transcript, Note = "No audio returned from Nova." });

            return File(result.AudioData, "audio/wav");
        }

    }
}
