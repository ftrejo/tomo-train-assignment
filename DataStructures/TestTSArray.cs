using NUnit.Framework;

using System;
using System.Collections.Generic;

using TrainSchedule.Database;
using TrainSchedule.Utils;

namespace TrainSchedule.DataStructures
{
    [TestFixture]
    public class TestTSArray
    {
        public TSArray tsArray;

        [SetUp]
        public void Init()
        {
            TrainlineEntity tle0 = new TrainlineEntity("ABCD", "10:00");
            TrainlineEntity tle1 = new TrainlineEntity("1001", "20:10");
            TrainlineEntity tle2 = new TrainlineEntity("qqwe", "00:00,11:00");

            List<TrainlineEntity> tles = new List<TrainlineEntity>();
            tles.Add(tle0);
            tles.Add(tle1);
            tles.Add(tle2);
            tsArray = new TSArray(tles);
        }

        [Test]
        public void TestGetNextTimeMutlipleTrains()
        {
            string nextTime = tsArray.GetNextTimeMutlipleTrains(0);
            Assert.True(nextTime.Equals("Multiple trains do not arrive at the same time"));

            tsArray[Utility.ConvertTimeToInt("11:00")] = 2;
            nextTime = tsArray.GetNextTimeMutlipleTrains(661);
            Assert.AreEqual("11:00 AM", nextTime);

            Assert.Throws<IndexOutOfRangeException>(() => tsArray.GetNextTimeMutlipleTrains(1440));
        }

        [Test]
        public void TestTSArrayAccessor()
        {
            Assert.AreEqual(1, tsArray[600]);
            Assert.AreEqual(0, tsArray[10]);
            tsArray[10] += 1;
            Assert.AreEqual(1, tsArray[10]);
        }
    }
}
