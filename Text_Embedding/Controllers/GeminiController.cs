using GeminiIntegration.ModelServices;
using GeminiIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GeminiIntegration.Interface;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace GeminiIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly ITableStorage<ChatHistory> _chatHistoryTable;
        private readonly ITableStorage<ChatEmbedding> _chatEmbeddingTable;

        public GeminiController(
            IGeminiService geminiService,
            Func<ITableStorage<ChatHistory>> chatHistoryTable,
            Func<ITableStorage<ChatEmbedding>> chatEmbeddingTable)
        {
            _geminiService = geminiService;
            _chatHistoryTable = chatHistoryTable.Invoke();
            _chatEmbeddingTable = chatEmbeddingTable.Invoke();
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContent([FromBody] GeminiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt cannot be empty.");

            var result = await _geminiService.GenerateContentAsync(request.Prompt);
            var embedding = await _geminiService.GenerateEmbeddingAsync(request.Prompt);

            ChatHistory chatHistory = new ChatHistory
            {
                Request = request.Prompt,
                Response = result
            };
            await _chatHistoryTable.UpsertAsync(chatHistory);

           

            ChatEmbedding chatEmbedding = new ChatEmbedding
            {
                Prompt = request.Prompt,
                Embedding = JsonSerializer.Serialize(embedding)
            };

            await _chatEmbeddingTable.UpsertAsync(chatEmbedding);

            return Ok(new GeminiResponse { Content = result });
        }

        [HttpGet("RetriveData")]
        public async Task<IActionResult> RetrieveData()
        {
            var chatHistoryList = await _chatHistoryTable.GetAsync();
            return Ok(chatHistoryList);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var result = await _chatHistoryTable.DeleteAsync(partitionKey, rowKey);
            if (!result)
                return NotFound("Record not found or already deleted.");
            return Ok("Record deleted successfully.");
        }

        [HttpDelete("DeleteBulk")]
        public async Task<IActionResult> DeleteBulk(string partitionKey)
        {
            var result = await _chatHistoryTable.DeleteBulkAsync(partitionKey);
            if (!result)
                return NotFound("No records found for the given partition key.");
            return Ok("Bulk delete successful.");
        }
    }
}
