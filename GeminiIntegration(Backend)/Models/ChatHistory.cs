using Azure;
using Azure.Data.Tables;

namespace GeminiIntegration.Models
{
    public class ChatHistory : ITableEntity
    {
        public string PartitionKey { get; set; } = "Chat";
        public string RowKey { get ; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get ; set; } = DateTimeOffset.UtcNow;
        public ETag ETag { get ; set; }

        public string Request { get; set; }
        public string Response { get;  set; }
    }
}
