using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Models
{
    public class TrainlineEntity : TableEntity
    {
        public string Schedule { get; set; }

        public TrainlineEntity()
        {

        }

        public TrainlineEntity(String name, String schedule)
        {
            Schedule = schedule;
            PartitionKey = "0000";
            RowKey = name;
        }
    }
}
