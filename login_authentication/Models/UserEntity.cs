using Azure;
//using login_authentication.Instances;
using Azure.Data.Tables;
using System;

namespace login_authentication.Models
{
    public class UserEntity: ITableEntity
    {
        public string PartitionKey { get; set; } = "Users";
        public string RowKey { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
