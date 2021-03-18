using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;

using TrainSchedule.Database;
using TrainSchedule.DataStructures;
using TrainSchedule.Models;
using TrainSchedule.Helpers;


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
        public void AddTrainline(TrainlineEntity trainline)
        {
            Store store = new Store();
            if (!Helper.IsValidName(trainline.Name))
                throw new Exception();

            List<string> times = new List<string>();
            foreach (string s in trainline.ScheduleArr)
                times.Add(Helper.FormatTime(s));

            string output = String.Join(",", times.Select(p => p.ToString()).ToArray());
            
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
            return Helper.ScheduleToJson(tle.Schedule);
        }

        [HttpGet]
        [Route("GetAllTrainlineNames")]
        public String GetAllTrainlineNames()
        {
            Store store = new Store();
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            FormatResponse(200);
            return Helper.FormatTrainNames(trainlines);
        }

        [HttpGet]
        [Route("GetNextMultipleTrains")]
        public String GetNextMultipleTrains(string time)
        {
            Store store = new Store();
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            TSArray tsa = new TSArray(trainlines);

            int targetIndex = Helper.ConvertToInt(Helper.FormatTime(time));
            tsa[targetIndex] = 2;

            return Helper.GetNextTimeMutlipleTrains(tsa, targetIndex);
        }

        private void FormatResponse(int code)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = code;
            
        }
    }
}
