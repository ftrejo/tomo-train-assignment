using System.Collections.Generic;
using System.Threading;

using TrainSchedule.Helpers;
using TrainSchedule.Models;

namespace TrainSchedule.DataStructures
{
    public class TSArray
    {
        private Data[] data = new Data[1440];

        public TSArray(List<TrainlineEntity> tle)
        {
            for(int i = 0; i < data.Length; i++)
            {
                data[i] = new Data();
            }

            populateArray(tle);
        }

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

    public class Data
    {
        public int count = 0;

        public void Increment()
        {
            count++;
        }
    }
}
