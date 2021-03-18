using System;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Database
{
    /// <summary>
    /// Trainline inherits from TableEntity which is the object to query against
    /// </summary>
    public class TrainlineEntity : TableEntity
    {
        public string Schedule { get; set; }

        public TrainlineEntity()
        {
            PartitionKey = "0000";
        }

        public TrainlineEntity(String name, String schedule)
        {
            Schedule = schedule;
            PartitionKey = "0000";
            RowKey = name;
        }
    }
}
