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
        Random rand = new Random();
        while (true)
        {
            lock (Question1.queue)
            {
                if (Question1.queue.q.Count < Question1.queue.Limit)
                {
                    Question1.queue.Enqueue(rand.Next());
                }
            }

            Question1.printQueue();
            Thread.Sleep(10);
        }
    }


}

public class Consumer
{
    public void run()
    {
        while (true)
        {
            lock (Question1.queue)
            {
                if (Question1.queue.q.Count > 0)
                {
                    Question1.queue.Dequeue();
                }
            }
            Question1.printQueue();
            Thread.Sleep(10);
        }
    }
}

public class ConcurrentQueueFixedSize<T>
{
    public ConcurrentQueue<T> q = new ConcurrentQueue<T>();
    
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
            lock(queue){
                int[] array = queue.q.ToArray();
                Console.WriteLine("Printing out the queue");
                for (int i = 0; i < queue.q.Count; i++)
                {
                    Console.WriteLine(array[i]);
                }

                //if (queue.q.Count > 9)
                //{
                    //Console.WriteLine("OVER 9 ELEMENTS");
                //}

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

            //while (!thread.IsAlive) ; // spin and wait



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
