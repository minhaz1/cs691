using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;


namespace Assignment1
{
public class Producer
{
    public void run()
    {
        // random number generator to insert rand values
        Random rand = new Random();
        while (true)
        {
            // lock the queue 
            lock (Question1.queue)
            {
                // check if it has reached capacity before inserting
                if (Question1.queue.q.Count < Question1.queue.Limit)
                {
                    Question1.queue.Enqueue(rand.Next());
                }
            }

            // print queue and sleep
            Question1.printQueue();
            Thread.Sleep(10);
        }
    }


}

public class Consumer
{
    public void run()
    {
        // infinitely add stuff
        while (true)
        {
            // lock the queue
            lock (Question1.queue)
            {
                // dequeue if queue is not empty
                if (Question1.queue.q.Count > 0)
                {
                    Question1.queue.Dequeue();
                }
            }
            // print out the queue after adding
            Question1.printQueue();
            Thread.Sleep(10);
        }
    }
}

// wrapper class on top of concurrent queue
public class ConcurrentQueueFixedSize<T>
{
    public ConcurrentQueue<T> q = new ConcurrentQueue<T>();
    
    // limit of the queue (set in main)
    public int Limit { get; set; }
    public void Enqueue(T obj)
    {
        q.Enqueue(obj);
    }

    public T Dequeue()
    {
        T temp;
        q.TryDequeue(out temp);
        return temp;
    }

}


    public class Question1
    {
        public static ConcurrentQueueFixedSize<int> queue = new ConcurrentQueueFixedSize<int>();

        public static void printQueue()
        {
            // lock the queue while it's printing, to get an accurate picture
            lock(queue){
                int[] array = queue.q.ToArray();
                Console.WriteLine("Printing out the queue");
                for (int i = 0; i < queue.q.Count; i++)
                {
                    Console.WriteLine(array[i]);
                }

            }
        }   


        static void Main(string[] args)
        {

            queue.Limit = 10;
            
            for (int i = 0; i < 10; i++)
            {
                new Thread(new ThreadStart(new Producer().run)).Start();
                new Thread(new ThreadStart(new Consumer().run)).Start();
            }

            Console.WriteLine();
            Console.WriteLine("Producer.run has finished");

            try
            {
                Console.WriteLine("Trying to restart the thread");
            }
            catch(ThreadStateException)
            {
                Console.Write("ThreadStateException trying to restart Alpha.Beta. ");
                Console.WriteLine("Expected since aborted threads cannot be restarted.");
                Console.Read();
            }

            Console.Read();


        }
    }
}
