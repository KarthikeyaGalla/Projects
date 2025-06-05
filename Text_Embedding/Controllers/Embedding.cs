using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Text_Embedding.Models;
using Text_Embedding.Interface;

namespace Text_Embedding.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbeddingController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly TableClient _tableClient;

        // Constructor: Injecting dependencies and setting up the TableClient
        public EmbeddingController(IGeminiService geminiService, IConfiguration configuration)
        {
            _geminiService = geminiService;

            var storageConnectionString = configuration.GetConnectionString("AzureTableStorage");
            var tableName = configuration["AzureTableSettings:TableName"];

            _tableClient = new TableClient(storageConnectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        // POST method to create and store the embedding
        [HttpPost]
        public async Task<IActionResult> CreateEmbedding([FromBody] string prompt)
        {
            try
            {
                var embedding = await _geminiService.GenerateEmbeddingAsync(prompt);

                var embeddingJson = JsonSerializer.Serialize(embedding);

                var entity = new PromptEmbeddingEntity
                {
                    PartitionKey = "Prompt",
                    RowKey = Guid.NewGuid().ToString(),
                    Prompt = prompt,
                    EmbeddingJson = embeddingJson
                };

                await _tableClient.AddEntityAsync(entity);

                return Ok("Embedding stored successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
