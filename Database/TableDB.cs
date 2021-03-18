using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Database
{
    public class TableDB
    {
        private CloudTable _table;
        public TableDB(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
        }

        public async void Set<T>(string key, object value) where T : TableEntity
        {
            ITableEntity entity = (T)value;

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                await _table.ExecuteAsync(insertOrMergeOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<TableResult> Fetch<T>(string key) where T : TableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>("0000", key);

            return await _table.ExecuteAsync(retrieveOperation);
        }

        public async Task<List<T>> Keys<T>() where T : TableEntity, new()
        {
            List<T> tableEntities = new List<T>();

            TableQuery<T> query = new TableQuery<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var page = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = page.ContinuationToken;
                tableEntities.AddRange(page.Results);
            }
            while (continuationToken != null);

            return tableEntities;
        }
    }
}
