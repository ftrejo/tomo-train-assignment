using System;
namespace TrainSchedule.Base
{
    public class ThreadSafeArray
    {
        private Count[] count = new Count[1440];
        public ThreadSafeArray()
        {
            for(int i = 0; i < count.Length; i++)
            {
                count[i] = new Count();
            }
        }

        public void LockAndIncrement(int index)
        {
            lock(count[index])
            {
                count[index].Increment();
            }
        }

        public void printArray()
        {
            for(int i = 0; i < 1440; i++)
            {
                Console.WriteLine(i + " " + this[i]);
            }
        }

        public int this[int id]
        {
            get
            {
                return count[id].freq;
            }
            set
            {
                count[id].freq = value;
            }
        }
    }

    public class Count
    {
        public int freq = 0;

        public void Increment()
        {
            freq++;
        }
    }
}
