using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Models
{
    public class TrainlineEntity : TableEntity
    {
        [Required]
        public String Name { get; set; }

        [IgnoreDataMember]
        public string Schedule { get; set; }

        public string[] ScheduleArr { get; set; }

        public TrainlineEntity()
        {
        }

        public TrainlineEntity(String name, String schedule)
        {
            Schedule = schedule;
            PartitionKey = "0000";
            RowKey = name;
            Name = name;
        }
    }
}
