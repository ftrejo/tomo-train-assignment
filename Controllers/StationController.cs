using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;

using TrainSchedule.Config;
using TrainSchedule.Database;
using TrainSchedule.DataStructures;
using TrainSchedule.Models;
using TrainSchedule.Utils;

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
        public StationController()
        {
            _connectionString = CosmosTableDB.GetConnectionString();
        }

        /// <summary>
        /// Add a trainline to the DB
        /// </summary>
        /// <param name="trainline">Trainline from post request</param>
        [HttpPost]
        [Route("AddTrainline")]
        public IActionResult AddTrainline(Trainline trainline)
        {
            try
            {
                TableDB store = new TableDB(_connectionString, _tableName);
                if (!Utility.IsValidName(trainline.Name))
                {
                    throw new Exception(string.Format("{0} is not a valid name.", trainline.Name));
                }

                TrainlineEntity tle = new TrainlineEntity();

                List<string> times = new List<string>();
                foreach (string s in trainline.Schedule)
                    times.Add(Utility.FormatTime(s));

                tle.Schedule = String.Join(",", times.Select(p => p.ToString()).ToArray());

                store.Set<TrainlineEntity>(trainline.Name, tle);
                return formatResponse(new Trainline(tle), 201, "application/json");

            }
            catch (Exception ex)
            {
                return formatResponse(
                    string.Format("AddTrainline error: {0}", ex.Message),
                    (int)HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Retrieve a Trainline object from its key
        /// </summary>
        /// <param name="key">Trainline key</param>
        /// <returns>Trainline object</returns>
        [HttpGet]
        [Route("GetTrainline")]
        public IActionResult GetTrainline(string key)
        {
            try
            {
                TableDB store = new TableDB(_connectionString, _tableName);
                Task<TableResult> taskTableResult = store.Fetch<TrainlineEntity>(key);
                TableResult tableResult = taskTableResult.Result;
                TrainlineEntity tle = (TrainlineEntity)tableResult.Result;

                if (tle == null)
                    throw new Exception(string.Format("key \"{0}\" not found", key));

                return formatResponse(new Trainline(tle), tableResult.HttpStatusCode, "application/json");
            }
            catch(Exception ex)
            {
                return formatResponse(
                    string.Format("GetTrainline error: {0}", ex.Message),
                    (int)HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Get all trainline names
        /// </summary>
        /// <returns>String of trainline names</returns>
        [HttpGet]
        [Route("GetAllTrainlineNames")]
        public IActionResult GetAllTrainlineNames()
        {
            try
            {
                TableDB store = new TableDB(_connectionString, _tableName);
                Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
                List<TrainlineEntity> trainlines = taskTrainlines.Result;

                if(trainlines.Count == 0)
                    return formatResponse("No trainlines", 200, "application/json");
                else
                    return formatResponse(Utility.FormatTrainNamesJson(trainlines), 200, "application/json");
            }
            catch(Exception ex)
            {
                return formatResponse(
                    string.Format("GetAllTrainlineName error: {0}", ex.Message),
                    (int)HttpStatusCode.BadRequest);
            }
            
        }

        /// <summary>
        /// Get the next time multiple trains arrive at the station
        /// after the input time
        /// </summary>
        /// <param name="time">Time to compare against</param>
        /// <returns>String with the next time multiple trains arrive</returns>
        [HttpGet]
        [Route("GetNextMultipleTrains")]
        public IActionResult GetNextMultipleTrains(string time)
        {
            try
            {
                TableDB store = new TableDB(_connectionString, _tableName);
                Task<List<TrainlineEntity>> taskTrainlines = store.Keys<TrainlineEntity>();
                List<TrainlineEntity> trainlines = taskTrainlines.Result;

                TSArray tsArr = new TSArray(trainlines);

                int targetIndex = Utility.ConvertTimeToInt(Utility.FormatTime(time));
                string result = tsArr.GetNextTimeMutlipleTrains(targetIndex);

                return formatResponse(tsArr.GetNextTimeMutlipleTrains(targetIndex), 200, "application/json");
            }
            catch (Exception ex)
            {
                return formatResponse(
                    string.Format("GetNextMultipleTrains error: {0}", ex.Message),
                    (int)HttpStatusCode.BadRequest);
            }
        }

        private JsonResult formatResponse(object value, int code, string contentType = "text/html")
        {
            JsonResult result = new JsonResult(value);
            result.StatusCode = code;
            result.ContentType = contentType;

            return result;
        }
    }
}
