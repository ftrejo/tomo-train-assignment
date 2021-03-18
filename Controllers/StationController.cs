using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.AspNetCore.Http;

using TrainSchedule.Models;
using TrainSchedule.Base;
using TrainSchedule;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Threading;

namespace TrainSchedule.Controllers
{
    [ApiController]
    [Route("api/station")]
    public class StationController : ControllerBase
    {
        public StationController()
        {
        }

        [HttpPost]
        [Route("AddTrainline")]
        public void AddTrainline(Trainline trainline)
        {
            if (!this.isValidName(trainline.Name))
                throw new Exception();

            List<string> times = new List<string>();
            foreach (string s in trainline.Schedule)
                times.Add(formatTime(s));

            string output = String.Join(",", times.Select(p => p.ToString()).ToArray());
            Store store = new Store();
            int code = store.Set(trainline.Name, output).Result.HttpStatusCode;

            FormatResponse(code);
            return;
        }

        [HttpGet]
        [Route("GetTrainline")]
        public String GetTrainline(string key)
        {
            Store store = new Store();
            Task<TableResult> taskTableResult = store.Fetch(key);
            TableResult tableResult = taskTableResult.Result;
            TrainlineEntity tle = (TrainlineEntity)tableResult.Result;

            int code = tableResult.HttpStatusCode;
            FormatResponse(code);
            return toJson(tle.Schedule);
        }

        [HttpGet]
        [Route("GetAllTrainlineNames")]
        public String GetAllTrainlineNames()
        {
            Store store = new Store();
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            //int code = tableResult.HttpStatusCode;
            FormatResponse(200);
            return formatTrainNames(trainlines);
        }

        [HttpGet]
        [Route("GetNextMultipleTrains")]
        public String GetNextMultipleTrains(string time)
        {
            Store store = new Store();
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            ThreadSafeArray tsa = new ThreadSafeArray();

            List<Thread> threads = new List<Thread>();
            foreach (TrainlineEntity tle in trainlines)
            {
                Thread childThread = new Thread(() => populateArray(tsa, tle));

                childThread.Start();
                threads.Add(childThread);
            }

            foreach (Thread thread in threads)
                thread.Join();


            //tsa.printArray();

            int targetIndex = convertToInt(formatTime(time));
            tsa[targetIndex] = 2;

            bool isDone = false;
            int index = targetIndex;
            while(!isDone)
            {
                index++;

                if (index == targetIndex)
                    isDone = true;

                if (index == 1440)
                    index = 0;

                if(tsa[index] > 1)
                {
                    return convertToString(index);
                }
            }


            return "NA";
        }

        private bool isValidName(string pName)
        {
            return pName.Length <= 4 && Regex.IsMatch(pName, "^[a-zA-Z0-9]*$");
        }

        private string formatTime(string dts)
        {
            DateTime dt = DateTime.Parse(dts);
            return dt.ToString("HH:mm");
        }

        private void FormatResponse(int code)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = code;
            
        }

        private string toJson(string schedule)
        {
            string json = "[";

            string[] times = schedule.TrimStart('[').TrimEnd(']').Split(',');
            foreach(string s in times)
            {
                String.Format("'{0}',", s);
                json += String.Format("'{0}', ", s);
            }

            json += "]";

            return json;
        }

        private string formatTrainNames(List<TrainlineEntity> tles)
        {
            string json = "[";
            foreach(TrainlineEntity tle in tles)
            {
                json += String.Format("'{0}', ", tle.RowKey);
            }
            json += "]";

            return json;
        }

        private void populateArray(ThreadSafeArray tsa, TrainlineEntity tle)
        {
            string[] times = tle.Schedule.TrimStart('[').TrimEnd(']').Split(',');
            foreach (string s in times)
            {
                int index = convertToInt(s);

                tsa.LockAndIncrement(index);
            }
        }

        private int convertToInt(string hourMinute)
        {
            string[] time = hourMinute.Split(":");

            return (Convert.ToInt32(time[0]) * 60) + Convert.ToInt32(time[1]);
        }

        private string convertToString(int index)
        {
            return String.Format("{0}:{1}", index / 60, index % 60);
        }
    }
}
