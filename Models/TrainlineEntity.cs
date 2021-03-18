using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Models
{
    /// <summary>
    /// Trainline inherits from TableEntity which is the object to query against
    /// </summary>
    public class TrainlineEntity : TableEntity
    {
        private string _name;
        /// <summary>
        /// Trainline name
        /// </summary>
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
