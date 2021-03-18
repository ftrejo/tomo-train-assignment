using System.Collections.Generic;
using System.Threading;

using TrainSchedule.Helpers;
using TrainSchedule.Database;

namespace TrainSchedule.DataStructures
{
    /// <summary>
    /// Thread Safe Array
    /// </summary>
    public class TSArray
    {
        private Data[] data = new Data[1440];

        /// <summary>
        /// Initializes the TSArray with a list of TrainlineEntities
        /// </summary>
        /// <param name="tle">List of trainlineEntities</param>
        public TSArray(List<TrainlineEntity> tle)
        {
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = new Data();
            }

            populateArray(tle);
        }

        /// <summary>
        /// Array accessor
        /// </summary>
        /// <param name="id">Index</param>
        /// <returns>Number of times the index has been visited</returns>
        public int this[int id]
        {
            get
            {
                return data[id].count;
            }
            set
            {
                data[id].count = value;
            }
        }

        /// <summary>
        /// populateArray takes the List of TrainlineEntities and updates the value
        /// of the array. This method uses threads to add to the array in parallel
        /// </summary>
        private void populateArray(List<TrainlineEntity> trainlines)
        {
            List<Thread> threads = new List<Thread>();
            foreach (TrainlineEntity tle in trainlines)
            {
                Thread childThread = new Thread(() => populateData(tle));

                childThread.Start();
                threads.Add(childThread);
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        /// <summary>
        /// populateData converts the times to an integer and then locks the
        /// object at that integer. We increment the value of the object
        /// </summary>
        /// <param name="tle">TrainlineEntity to add to the array</param>
        private void populateData(TrainlineEntity tle)
        {
            string[] times = tle.Schedule.TrimStart('[').TrimEnd(']').Split(',');
            foreach (string s in times)
            {
                int index = Helper.ConvertTimeToInt(s);

                lock (data[index])
                {
                    data[index].Increment();
                }
            }
        }
    }

    /// <summary>
    /// Simple object that stores a count
    /// </summary>
    public class Data
    {
        public int count = 0;

        public void Increment()
        {
            count++;
        }
    }
}
