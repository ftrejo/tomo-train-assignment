using NUnit.Framework;

using System;
using System.Collections.Generic;

using TrainSchedule.Database;
using TrainSchedule.Utils;

namespace TrainSchedule.Tests
{
    [TestFixture]
    public class TestUtility
    {
        [Test]
        public void TestConvertIndexToTime()
        {
            Assert.AreEqual("12:00 AM", Utility.ConvertIndexToTime(0));
            Assert.AreEqual("11:59 PM", Utility.ConvertIndexToTime(1439));
            Assert.AreEqual("03:01 PM", Utility.ConvertIndexToTime(901));
            Assert.AreEqual("Invalid", Utility.ConvertIndexToTime(-1));
        }

        [Test]
        public void TestConvertTimeToInt()
        {
            Assert.AreEqual(0, Utility.ConvertTimeToInt("00:00"));
            Assert.AreEqual(1439, Utility.ConvertTimeToInt("23:59"));
        }

        [Test]
        public void TestFormatTrainNames()
        {
            TrainlineEntity tle0 = new TrainlineEntity("ABCD", "10:00");
            TrainlineEntity tle1 = new TrainlineEntity("1001", "20:10");
            TrainlineEntity tle2 = new TrainlineEntity("qqwe", "00:00,2:00");

            List<TrainlineEntity> tles = new List<TrainlineEntity>();
            tles.Add(tle0);
            tles.Add(tle1);
            tles.Add(tle2);

            string trainsJson = Utility.FormatTrainNamesJson(tles);

            Assert.True(trainsJson.Contains("ABCD"));
            Assert.True(trainsJson.Contains("1001"));
            Assert.True(trainsJson.Contains("qqwe"));
            Assert.True(trainsJson.Contains("10"));
        }

        [Test]
        public void TestIsValidName()
        {
            Assert.True(Utility.IsValidName("ABCD"));
            Assert.True(Utility.IsValidName("0001"));

            Assert.False(Utility.IsValidName("  "));
            Assert.False(Utility.IsValidName("!"));
            Assert.False(Utility.IsValidName("12345"));
        }
    }
}
