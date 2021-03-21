using System;
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
            string[] sched = tle.Schedule.TrimStart('[').TrimEnd(']').Split(',');
            for (int i = 0; i < sched.Length; i++)
            {
                sched[i] = DateTime.Parse(sched[i]).ToString("hh:mm tt");
            }

            Schedule = sched;
        }
    }
}
