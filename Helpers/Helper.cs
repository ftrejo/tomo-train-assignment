using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using TrainSchedule.DataStructures;
using TrainSchedule.Models;

namespace TrainSchedule.Helpers
{
    public static class Helper
    {
        public static string ConvertToString(int index)
        {
            string formatted = String.Format("{0}:{1}", index / 60, index % 60);
            return DateTime.Parse(formatted).ToString("hh:mm tt");
        }

        public static int ConvertToInt(string hourMinute)
        {
            string[] time = hourMinute.Split(":");

            return (Convert.ToInt32(time[0]) * 60) + Convert.ToInt32(time[1]);
        }

        public static string FormatTrainNames(List<TrainlineEntity> tles)
        {
            string json = "[";
            foreach (TrainlineEntity tle in tles)
            {
                json += String.Format("'{0}', ", tle.RowKey);
            }
            json += "]";

            return json;
        }

        public static string ScheduleToJson(string schedule)
        {
            string json = "[";

            string[] times = schedule.TrimStart('[').TrimEnd(']').Split(',');
            foreach (string s in times)
            {
                String.Format("'{0}',", s);
                json += String.Format("'{0}', ", s);
            }

            json += "]";

            return json;
        }

        public static string FormatTime(string dts)
        {
            DateTime dt = DateTime.Parse(dts);
            return dt.ToString("HH:mm");
        }

        public static bool IsValidName(string pName)
        {
            return pName.Length <= 4 && Regex.IsMatch(pName, "^[a-zA-Z0-9]*$");
        }

        public static String GetNextTimeMutlipleTrains(TSArray tsa, int targetIndex)
        {
            bool isDone = false;
            int index = targetIndex;

            while (!isDone)
            {
                index++;

                if (index == targetIndex)
                    isDone = true;

                if (index == 1440)
                    index = 0;

                if (tsa[index] > 1)
                {
                    return ConvertToString(index);
                }
            }

            return "Multiple trains do not arrive at the same time";
        }
    }
}
