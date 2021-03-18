using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainSchedule.Models;

namespace TrainSchedule.Database
{
    public class Store
    {
        private CloudTable table;
        public Store()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=trainschedulecosmos;AccountKey=tLeW8xP7uBdLxKCJIh9GQXCfCvVAjkdD15ucEwnSEhcYdWHGkvpSCuc4FHyPMKKeXQMMkIuh66j8Ae3fc02dVw==;TableEndpoint=https://trainschedulecosmos.table.cosmos.azure.com:443/;";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("schedules");
        }

        public async Task<TableResult> Set(string key, object value)
        {
            TrainlineEntity trainlineEntity = new TrainlineEntity(key, (string)value);

            return await InsertOrMergeEntityAsync(table, trainlineEntity);
        }

        public async Task<TableResult> Fetch(string key)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TrainlineEntity>("0000", key);

            return await table.ExecuteAsync(retrieveOperation);
        }

        public async Task<List<TrainlineEntity>> Keys()
        {
            List<TrainlineEntity> trainlines = new List<TrainlineEntity>();

            TableQuery<TrainlineEntity> query = new TableQuery<TrainlineEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var page = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = page.ContinuationToken;
                trainlines.AddRange(page.Results);
            }
            while (continuationToken != null);

            return trainlines;
        }

        private static async Task<TableResult> InsertOrMergeEntityAsync(CloudTable table, TableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                return await table.ExecuteAsync(insertOrMergeOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
