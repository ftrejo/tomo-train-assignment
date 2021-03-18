using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

using TrainSchedule.Config;
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
        private string _connectionString;
        private const string _tableName = "schedules";
        public StationController(IOptions<CosmosTableDB> settings)
        {
            _connectionString = settings.Value.ConnectionString;
        }

        [HttpPost]
        [Route("AddTrainline")]
        public void AddTrainline(TrainlineEntity trainline)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            if (!Helper.IsValidName(trainline.Name))
            {
                //Response.
                throw new HttpRequestException("Invalid Name", null,
                    System.Net.HttpStatusCode.BadRequest);
            }

            List<string> times = new List<string>();
            foreach (string s in trainline.ScheduleArr)
                times.Add(Helper.FormatTime(s));

            string formattedStr = String.Join(",", times.Select(p => p.ToString()).ToArray());
            trainline.Schedule = formattedStr;
            try
            {
                store.Set<TrainlineEntity>(trainline.Name, trainline);
                FormatResponse(204);
            }
            catch(Exception ex)
            {
                throw new HttpRequestException("AddTrainline Table DB error", ex,
                    System.Net.HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [Route("GetTrainline")]
        public String GetTrainline(string key)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<TableResult> taskTableResult = store.Fetch<TrainlineEntity>(key);
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
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            FormatResponse(200);
            return Helper.FormatTrainNames(trainlines);
        }

        [HttpGet]
        [Route("GetNextMultipleTrains")]
        public String GetNextMultipleTrains(string time)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            TSArray tsa = new TSArray(trainlines);

            int targetIndex = Helper.ConvertToInt(Helper.FormatTime(time));

            return Helper.GetNextTimeMutlipleTrains(tsa, targetIndex);
        }

        private void FormatResponse(int code)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = code;
            
        }
    }
}
