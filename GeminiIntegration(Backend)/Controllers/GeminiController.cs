using GeminiIntegration.ModelServices;
using GeminiIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GeminiIntegration.Interface;
using System.Text.RegularExpressions;

namespace GeminiIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService  _geminiService;
        private readonly ITableStorage<ChatHistory>  _chatHistorytable;


        public GeminiController(IGeminiService geminiService, Func<ITableStorage<ChatHistory>> chatHistorytable)
        {
            _geminiService = geminiService;
            _chatHistorytable = chatHistorytable.Invoke();

        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContent([FromBody] GeminiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt cannot be empty.");

            var result = await _geminiService.GenerateContentAsync(request.Prompt);

            ChatHistory data = new ChatHistory()
            {
                Request = request.Prompt,
                Response = result
            };

            var x = await _chatHistorytable.UpsertAsync(data);

            return Ok(new GeminiResponse { Content = result });
        }

        [HttpGet("RetriveData")]
        public async Task<IActionResult> RetrieveData()
        {
            var chatHistoryList = await _chatHistorytable.GetAsync();
            return Ok(chatHistoryList);
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var result = await _chatHistorytable.DeleteAsync(partitionKey, rowKey);

            if (!result)
                return NotFound($"Record with {result} not found or already deleted.");

            return Ok($"{result} Record deleted successfully.");
        }

        // DELETE all records under the same partition key
        [HttpDelete("DeleteBulk")]
        public async Task<IActionResult> DeleteBulk(string partitionKey)
        {
            var result = await _chatHistorytable.DeleteBulkAsync(partitionKey);

            if (!result)
                return NotFound("No records found for the given partition key.");

            return Ok("Bulk delete successful.");
        }
    }
}
