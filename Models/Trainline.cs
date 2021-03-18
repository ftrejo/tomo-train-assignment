using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using TrainSchedule.Database;

/// <summary>
/// REST API model for trainlines
/// </summary>
namespace TrainSchedule.Models
{
    public class Trainline
    {
        [Required]
        public String Name { get; set; }

        public string[] Schedule { get; set; }

        public Trainline()
        {
        }

        public Trainline(TrainlineEntity tle)
        {
            Name = tle.RowKey;

            Schedule = tle.Schedule.TrimStart('[').TrimEnd(']').Split(',');
        }
    }
}
