using Azure.Data.Tables;
using SpeechBot.Models;

public class AzureStorageService
{
    private readonly TableClient _tableClient;

    public AzureStorageService(IConfiguration config)
    {
        var connectionString = config["AzureStorage:ConnectionString"];
        var tableName = config["AzureStorage:TableName"];
        _tableClient = new TableClient(connectionString, tableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task SaveTranscriptAsync(string prompt, string response)
    {
        var entry = new TranscriptEntity { PromptText = prompt, ResponseText = response };
        await _tableClient.AddEntityAsync(entry);
    }
}
