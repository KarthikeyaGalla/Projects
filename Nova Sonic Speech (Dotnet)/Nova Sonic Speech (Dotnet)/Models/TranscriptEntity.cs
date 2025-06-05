using Azure;
using Azure.Data.Tables;

namespace SpeechBot.Models
{
    public class TranscriptEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Transcript";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public string PromptText { get; set; }
        public string ResponseText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ETag ETag { get; set; } = ETag.All;
        public DateTimeOffset? Timestamp { get; set; }
    }
}
