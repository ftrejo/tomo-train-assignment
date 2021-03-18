using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Table;

namespace TrainSchedule.Models
{
    public class Trainline
    {
        [Required]
        public String Name { get; set; }

        public string[] Schedule { get; set; }

        public Trainline(String name, string[] schedule)
        {
            Name = name;
            Schedule = schedule;
        }
    }
}
