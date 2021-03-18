using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Database
{
    /// <summary>
    /// TableDB handles database requests
    /// </summary>
    public class TableDB
    {
        private CloudTable _table;

        /// <summary>
        /// TableDB constructor
        /// </summary>
        /// <param name="connectionString">DB Connection string</param>
        /// <param name="tableName">Table to query against</param>
        public TableDB(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
        }

        /// <summary>
        /// Inserts an object that inherits from TableEntity into the DB
        /// </summary>
        /// <typeparam name="T">Class that extends TableEntity</typeparam>
        /// <param name="key">Key to add to the table</param>
        /// <param name="value">A TableEntity object</param>
        public async void Set<T>(string key, object value) where T : TableEntity
        {
            ITableEntity entity = (T)value;
            entity.RowKey = key;

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

        /// <summary>
        /// Fatch a row from the DB
        /// </summary>
        /// <typeparam name="T">Class that extends TableEntity</typeparam>
        /// <param name="key">Key to query against the tablee</param>
        /// <returns>TableEntity object from the DB</returns>
        public async Task<TableResult> Fetch<T>(string key) where T : TableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>("0000", key);

            return await _table.ExecuteAsync(retrieveOperation);
        }

        /// <summary>
        /// Get all the keys from the DB table
        /// </summary>
        /// <typeparam name="T">Type that extends TableEntity</typeparam>
        /// <returns><List of TableEntity keys/returns>
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
