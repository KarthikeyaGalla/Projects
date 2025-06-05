using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace GeminiIntegration.Models
{
    public class ChatEmbedding : ITableEntity
    {
        public string PartitionKey { get; set; } = "Embeddings";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public string Prompt { get; set; }
        public string Embedding { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

}
