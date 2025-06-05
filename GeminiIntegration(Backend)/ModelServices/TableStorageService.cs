
using Azure.Data.Tables;
using GeminiIntegration.Interface;
//using static GeminiIntegration.ModelServices.TableStorageService;

namespace GeminiIntegration.ModelServices
{
    public class TableStorageService<T> : ITableStorage<T> where T : class, ITableEntity, new()
    {
        private readonly TableClient client;
        private readonly ILogger logger;

        public TableStorageService(TableServiceClient tableServiceClient, string tableName, LoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<TableStorageService<T>>();
            client = tableServiceClient.GetTableClient(tableName);
        }


        public async Task<List<T>> GetAsync()
        {
            var response = client.QueryAsync<T>().AsPages();
            var list = new List<T>();
            await foreach (var page in response)
            {
                foreach (var item in page.Values)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public async Task<List<T>> GetAsync(string partitionKey)
        {
            var response = client.QueryAsync<T>(x => x.PartitionKey == partitionKey).AsPages();
            var list = new List<T>();
            await foreach (var page in response)
            {
                foreach (var item in page.Values)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public async Task<T?> GetAsync(string pKey, string rKey)
        {
            try
            {
                var response = await client.GetEntityAsync<T>(pKey, rKey);
                return response.Value;
            }
            catch (Exception rfEx)
            {
                logger.LogError("Resource not found.", rfEx);
                // return null;
                throw;
            }
        }

        public async Task<T>UpsertAsync(T item)
        {
            try
            {
                var response = await client.UpsertEntityAsync<T>(item, TableUpdateMode.Merge);
                return await GetAsync(item.PartitionKey, item.RowKey);
            }
            catch (Exception rfEx)
            {
                logger.LogError("Entity upsert failed", rfEx);
                throw new ApplicationException("Upsert failed", rfEx);
            }
        }

        public async Task<bool> DeleteAsync(string pKey, string rKey)
        {
            var response = await client.DeleteEntityAsync(pKey, rKey);
            if (!response.IsError)
            {
                return true;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<bool> DeleteBulkAsync(string pKey)
        {
            var response = true;

            var queryResult = client.QueryAsync<TableEntity>(x => x.PartitionKey == pKey, 100, new[] { "RowKey" });
            await foreach (Azure.Page<TableEntity> page in queryResult.AsPages())
            {
                Console.WriteLine("This is a new page!");
                foreach (TableEntity qEntity in page.Values)
                {
                    var resp = await client.DeleteEntityAsync(pKey, qEntity.RowKey);
                    response = !resp.IsError && response;
                }
            }

            return response;
        }

        public IEnumerable<T> Get(string filter)
        {
            return client.Query<T>(filter);
        }

        public async Task<List<T>> GetByDateAsync(DateTime fromDate, DateTime toDate)
        {
            var response = client.QueryAsync<T>(x => x.Timestamp >= fromDate && x.Timestamp <= toDate).AsPages();

            var list = new List<T>();

            await foreach (var page in response)
            {
                foreach (var item in page.Values)
                {
                    list.Add(item);
                }
            }

            return list;
        }
        public IEnumerable<T> GetAllRows()
        {
            return client.Query<T>();
        }


    }
}
