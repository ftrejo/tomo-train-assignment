using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ganss.XSS;

using TrainSchedule.Database;

namespace TrainSchedule.Utils
{
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Converts and int index to a time
        /// </summary>
        /// <param name="index">Index to convert</param>
        /// <returns>String time with the format (hh:mm tt) : (12:34 AM)</returns>
        public static string ConvertIndexToTime(int index)
        {
            if (index < 0 || index > 1439)
                return "Invalid";
            string formatted = String.Format("{0}:{1}", index / 60, index % 60);
            return DateTime.Parse(formatted).ToString("hh:mm tt");
        }

        /// <summary>
        /// Converts a string time to an int
        /// </summary>
        /// <param name="time">String time to be converted</param>
        /// <returns>Integer representation of a time</returns>
        public static int ConvertTimeToInt(string time)
        {
            string[] timeArr = time.Split(":");

            return (Convert.ToInt32(timeArr[0]) * 60) + Convert.ToInt32(timeArr[1]);
        }

        /// <summary>
        /// Convert a list of TrainlineEntities to json
        /// </summary>
        /// <param name="tles">List of TrainlineEntities </param>
        /// <returns>JSON representation of TrainlineEntities</returns>
        public static string FormatTrainNamesJson(List<TrainlineEntity> tles)
        {
            string json = "{";
            foreach (TrainlineEntity tle in tles)
            {
                json += String.Format("'{0}', ", tle.RowKey);
            }
            json = json.TrimEnd(' ').TrimEnd(',');
            json += "}";

            return json;
        }

        /// <summary>
        /// Formats a datetime string to (HH:mm)
        /// </summary>
        /// <param name="dts">Datetime string</param>
        /// <returns>String time</returns>
        public static string FormatTime(string dts)
        {
            DateTime dt = DateTime.Parse(dts);
            return dt.ToString("HH:mm");
        }

        /// <summary>
        /// Checks if a trainline name is valid
        /// A trainline name is valid when it is 4 characters long
        /// and a-z,A-Z,0-9
        /// </summary>
        /// <param name="name">Trainline name</param>
        /// <returns>Whether a trainline name is valid</returns>
        public static bool IsValidName(string name)
        {
            return name.Length <= 4 && Regex.IsMatch(name, "^[a-zA-Z0-9]*$");
        }

        public static string SanitizeInput(string input)
        {
            var sanitizer = new HtmlSanitizer();

            return sanitizer.Sanitize(input);
        }
    }
}
