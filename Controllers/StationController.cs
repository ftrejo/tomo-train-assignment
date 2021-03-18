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
    /// <summary>
    /// API controller to Add routes and get routes
    /// </summary>
    [ApiController]
    [Route("api/station")]
    public class StationController : ControllerBase
    {
        private string _connectionString;
        private const string _tableName = "schedules";

        /// <summary>
        /// Controller constructor with settings
        /// </summary>
        /// <param name="settings">Contains the connection string</param>
        public StationController(IOptions<CosmosTableDB> settings)
        {
            _connectionString = settings.Value.ConnectionString;
        }

        /// <summary>
        /// Add a trainline to the DB
        /// </summary>
        /// <param name="trainline">Trainline from post request</param>
        [HttpPost]
        [Route("AddTrainline")]
        public void AddTrainline(Trainline trainline)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            if (!Helper.IsValidName(trainline.Name))
            {
                //To do figure out how to return errors.
                throw new HttpRequestException("Invalid Name", null,
                    System.Net.HttpStatusCode.BadRequest);
            }

            TrainlineEntity tle = new TrainlineEntity();

            List<string> times = new List<string>();
            foreach (string s in trainline.Schedule)
                times.Add(Helper.FormatTime(s));

            tle.Schedule = String.Join(",", times.Select(p => p.ToString()).ToArray());

            try
            {
                store.Set<TrainlineEntity>(trainline.Name, tle);
                formatResponse(204);
            }
            catch(Exception ex)
            {
                throw new HttpRequestException("AddTrainline Table DB error", ex,
                    System.Net.HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Retrieve a Trainline object from its key
        /// </summary>
        /// <param name="key">Trainline key</param>
        /// <returns>Trainline object</returns>
        [HttpGet]
        [Route("GetTrainline")]
        public Trainline GetTrainline(string key)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<TableResult> taskTableResult = store.Fetch<TrainlineEntity>(key);
            TableResult tableResult = taskTableResult.Result;
            TrainlineEntity tle = (TrainlineEntity)tableResult.Result;

            int code = tableResult.HttpStatusCode;
            formatResponse(code);
            return new Trainline(tle);
        }

        /// <summary>
        /// Get all trainline names
        /// </summary>
        /// <returns>String of trainline names</returns>
        [HttpGet]
        [Route("GetAllTrainlineNames")]
        public String GetAllTrainlineNames()
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            formatResponse(200);
            return Helper.FormatTrainNamesJson(trainlines);
        }

        /// <summary>
        /// Get the next time multiple trains arrive at the station
        /// after the input time
        /// </summary>
        /// <param name="time">Time to compare against</param>
        /// <returns>String with the next time multiple trains arrive</returns>
        [HttpGet]
        [Route("GetNextMultipleTrains")]
        public String GetNextMultipleTrains(string time)
        {
            TableDB store = new TableDB(_connectionString, _tableName);
            Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
            List<TrainlineEntity> trainlines = taskTrainlines.Result;

            TSArray tsArr = new TSArray(trainlines);

            int targetIndex = Helper.ConvertTimeToInt(Helper.FormatTime(time));

            return Helper.GetNextTimeMutlipleTrains(tsArr, targetIndex);
        }

        private void formatResponse(int code)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = code;
            
        }
    }
}
