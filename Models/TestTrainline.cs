using NUnit.Framework;

using System;

using TrainSchedule.Database;

namespace TrainSchedule.Models
{
    [TestFixture]
    public class TestTrainline
    {
        [Test]
        public void TestTrainlineConstructor()
        {
            Trainline tl = new Trainline();
            tl.Name = "ABCD";
            tl.Schedule = new string[] { "11:30 AM", "6:02 PM" };

            Assert.AreEqual("ABCD", tl.Name);
            Assert.IsTrue(tl.Schedule.Length == 2);
            Assert.IsTrue(Array.Exists(tl.Schedule, element => element == "11:30 AM"));

            tl.Name = "ABC";
            Assert.AreEqual("ABC", tl.Name);
        }

        [Test]
        public void TestTrainlineConstructorFromTLE()
        {
            TrainlineEntity tle = new TrainlineEntity("1234", "11:30,18:02");
            Trainline tl = new Trainline(tle);
            tl.Schedule = new string[] { "11:30 AM", "6:02 PM" };

            Assert.AreEqual("1234", tl.Name);
            Assert.IsTrue(Array.Exists(tl.Schedule, element => element == "11:30 AM"));
        }
    }
}
