using Azure;
using Azure.Data.Tables;

namespace Text_Embedding.Models
{
    public class PromptEmbeddingEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }  
        public ETag ETag { get; set; }              

        public string Prompt { get; set; }
        public string EmbeddingJson { get; set; }
    }
}
