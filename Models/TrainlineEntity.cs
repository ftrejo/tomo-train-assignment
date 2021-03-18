using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Models
{
    public class TrainlineEntity : TableEntity
    {
        private string _name;
        [Required]
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                RowKey = value;
                _name = value;
            }
        }

        [IgnoreDataMember]
        public string Schedule { get; set; }

        public string[] ScheduleArr { get; set; }

        public TrainlineEntity()
        {
            PartitionKey = "0000";
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
