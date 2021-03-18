using System;

namespace TrainSchedule.Config
{
    /// <summary>
    /// Autopopulated class from appsetting.json files
    /// </summary>
    public class CosmosTableDB
    {
        /// <summary>
        /// DB connection string
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
