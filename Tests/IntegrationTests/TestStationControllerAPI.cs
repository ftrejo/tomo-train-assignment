using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

using System.Net;
using System.Threading;
using System.Threading.Tasks;

using TrainSchedule.Config;
using TrainSchedule.Models;
using TrainSchedule.Database;

namespace TrainSchedule.IntegrationTests
{
    [TestFixture]
    public class TestStationControllerAPI
    {
        private TableDB db;
        private RestClient client = new RestClient("https://localhost:5001/");

        [SetUp]
        public void Init()
        {
            db = new TableDB(CosmosTableDB.GetConnectionString(), "schedules");
            db.DeleteAll<TrainlineEntity>();
        }

        [Test]
        public async Task TestAddTrainline()
        {
            RestRequest request = new RestRequest("api/station/AddTrainline", DataFormat.Json);
            request.Method = Method.POST;

            Trainline tl = new Trainline();
            tl.Name = "ABCD";
            tl.Schedule = new string[] { "11:15 AM", "1:30 PM" };

            string result = JsonConvert.SerializeObject(tl);
            var cancellationTokenSource = new CancellationTokenSource();

            request.AddJsonBody(result);
            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task TestGetAll()
        {
            await createTrains();

            RestRequest request = new RestRequest("api/station/GetAllTrainlineNames", DataFormat.Json);
            request.Method = Method.GET;
            var cancellationTokenSource = new CancellationTokenSource();

            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.Content.Contains("ABCD"));
            Assert.IsTrue(response.Content.Contains("EFGH"));
            Assert.IsTrue(response.Content.Contains("IJKL"));
        }

        [Test]
        public async Task TestGetTrainline()
        {
            await createTrains();

            RestRequest request = new RestRequest("/api/station/GetTrainline", DataFormat.Json);
            request.Method = Method.GET;
            request.AddParameter("key", "ABCD");
            var cancellationTokenSource = new CancellationTokenSource();

            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.IsTrue(response.Content.Contains("11:15 AM"));
            Assert.IsTrue(response.Content.Contains("1:30 PM"));
        }

        [Test]
        public async Task TestGetMultipleTrains()
        {
            await createTrains();

            RestRequest request = new RestRequest("/api/station/GetNextMultipleTrains", DataFormat.Json);
            request.Method = Method.GET;
            request.AddParameter("time", "1:30 PM");
            var cancellationTokenSource = new CancellationTokenSource();

            var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.IsTrue(response.Content.Contains("2:30 PM"));
        }

        private async Task createTrains()
        {
            RestRequest request = new RestRequest("api/station/AddTrainline", DataFormat.Json);
            request.Method = Method.POST;

            // First item
            Trainline tl = new Trainline();
            tl.Name = "ABCD";
            tl.Schedule = new string[] { "11:15 AM", "1:30 PM" };

            string result = JsonConvert.SerializeObject(tl);
            var cancellationTokenSource = new CancellationTokenSource();

            request.AddJsonBody(result);
            await client.ExecuteAsync(request, cancellationTokenSource.Token);

            // Second item
            request = new RestRequest("api/station/AddTrainline", DataFormat.Json);
            request.Method = Method.POST;

            tl = new Trainline();
            tl.Name = "EFGH";
            tl.Schedule = new string[] { "12:15 AM", "2:30 PM" };

            result = JsonConvert.SerializeObject(tl);
            cancellationTokenSource = new CancellationTokenSource();

            request.AddJsonBody(result);
            await client.ExecuteAsync(request, cancellationTokenSource.Token);

            // Third item
            request = new RestRequest("api/station/AddTrainline", DataFormat.Json);
            request.Method = Method.POST;

            tl = new Trainline();
            tl.Name = "IJKL";
            tl.Schedule = new string[] { "1:15 PM", "2:30 PM" };

            result = JsonConvert.SerializeObject(tl);
            cancellationTokenSource = new CancellationTokenSource();

            request.AddJsonBody(result);
            await client.ExecuteAsync(request, cancellationTokenSource.Token);

        }
    }
}
