using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace ConsoleApplication1
{
    // declare delegate for computing sum concurrently
    public delegate int CountByPropertyDelegate(string name, string value);


    // temporary 
    public class Person
    {
        public string name { get; set; }

        public int age { get; set; }

        public bool bald { get; set; }

        public Person(string name, int age, bool bald)
        {
            this.name = name;
            this.age = age;
            this.bald = bald;
        }

        public void printName()
        {
            Console.WriteLine("Person2-name: " + name);
        }
    }

    // temporary person class for testing
    public class Person2
    {
        public string name { get; set; }

        public int age { get; set; }

        public bool bald { get; set; }

        public int Length { get; set; }

        public Person2(string name, int age, bool bald, int length)
        {
            this.name = name;
            this.age = age;
            this.bald = bald;
            this.Length = length;
        }

    }


    /** Wrapper class for thread to compute sum
     **/
    public class SumThread
    {
        CountByPropertyDelegate function;
        string propertyName;
        string propertyValue;
        public int count { get; set; }

        // constructor for wrapper. allows me to pass in the values to the function
        // and store the sum when it is done computing
        public SumThread(CountByPropertyDelegate function, string propertyName, string propertyValue)
        {
            this.function = function;
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;

        }

        // call the delegate function with the parameters and store the count
        public void run()
        {
            this.count = function(propertyName, propertyValue);
        }

    }


        public class Querier
        {
            IEnumerable<object> enumerables;

            public Querier(IEnumerable<object> collection)
            {
                enumerables = collection;
            }

            public int CountByProperty(string propertyName, string propertyValue)
            {

                List<object> list = enumerables.ToList();
                int count = 0;

                foreach (object obj in list)
                {
                    if (obj == null)
                        continue;

                    // get the property
                    PropertyInfo prop = obj.GetType().GetProperty(propertyName);

                    // if the property is null then it wasn't found, go to the next
                    if (prop == null)
                    {
                        continue;
                    }
                    
                    // it was found, so check the types
                    if (prop.PropertyType == typeof(String))
                    {
                        // after we confirm it is a string, cast the value to a string
                        string val = (string)prop.GetValue(obj, null);

                        // if it is equal, increment count
                        if (val == propertyValue)
                            count++;

                    }
                    else if(prop.PropertyType == typeof(int))
                    {
                        int val = (int)prop.GetValue(obj, null);

                        //after confirming int, parse string into int
                        if (val == int.Parse(propertyValue))
                            count++;

                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        bool val = (bool)prop.GetValue(obj, null);
                        if (val == bool.Parse(propertyValue))
                            count++;
                    }
                }

                return count;
            }

            public int SumByProperty(string propertyName1, string propertyValue1, string propertyName2, string propertyValue2)
            {
                // wrapper objects, pass in delegate and arguments
                SumThread one = new SumThread(CountByProperty, propertyName1, propertyValue1);
                SumThread two = new SumThread(CountByProperty, propertyName2, propertyValue2);

                // thread objects 
                Thread thread = new Thread(new ThreadStart(one.run));
                Thread thread2 = new Thread(new ThreadStart(two.run));


                thread.Start();
                thread2.Start();

                // wait for them both to finish
                while (thread.IsAlive || thread2.IsAlive) ;

                return one.count + two.count;
            }

            public int GroupByProperty(string propertyName)
            {
                // dictionary to store values. 
                // the values of the property will be used as keys and the values associated
                // with those keys will be incremented when another match is found.
                Dictionary<object, int> dict = new Dictionary<object, int>();

                // convert into list. may not be necessary
                List<object> list = enumerables.ToList();
                int count = 0;

                // iterate over all objects in the collection
                foreach (object obj in list)
                {
                    // if the object is null, skip everything
                    if (obj == null)
                        continue;


                    // get the property
                    PropertyInfo prop = obj.GetType().GetProperty(propertyName);

                    // if the property is null then it wasn't found, go to the next
                    if (prop == null)
                    {
                        count++;
                        continue;
                    }

                    // get the values and increment the value in the dictionary to keep count
                    object val = prop.GetValue(obj, null);
                    if (dict.ContainsKey(val))
                        dict[val]++;
                    else
                        dict[val] = 1;

                }

                // gets keys and sort them
                var keys = dict.Keys.ToList();
                keys.Sort();
                keys.Reverse();

                // print out the queue
                foreach (object obj in keys)
                {
                    Console.WriteLine(obj + "\t" + dict[obj]);
                }

                // return -1 if there are no matches
                return (count == 0) ? -1 : 1;
            }

            public int Transform(string functionName)
            {
                List<object> list = enumerables.ToList();
                int count = 0;

                foreach (object obj in list)
                {
                    if (obj == null)
                        continue;

                    // access the function
                    Type thisType = obj.GetType();
                    MethodInfo function = thisType.GetMethod(functionName);

                    // if function is found
                    if (function != null)
                    {
                        // invoke the function with no args
                        function.Invoke(obj, null);
                        count++;
                    }
                }

                // if count is 0, then there were no matches
                return (count == 0) ? -1: 1;
            }
        }


        class Question2
        {
            static void Main(string[] args)
            {

                object[] people = new object[12];

                people[0] = new Person("ben", 21, false);
                people[1] = new Person("ben", 21, false);
                people[2] = new Person("ben", 21, false);
                people[3] = new Person("ben3", 20, true);
                people[4] = new Person("ben2", 20, false);
                people[5] = new Person2("ben", 21, false, 5);
                people[6] = new Person2("ben", 21, false, 4);
                people[7] = new Person2("ben", 21, false, 8);
                people[8] = new Person2("ben3", 20, true, 5);
                people[9] = new Person2("ben2", 20, false, 9);
                people[10] = "hello";
                people[11] = "olleh";

                Querier q = new Querier(people);

                int count = q.CountByProperty("bald", "false");
                Console.WriteLine("Count: " + count);

                int length = q.CountByProperty("Length", "5");
                Console.WriteLine("length of 5 count: " + length);

                int sumcount = q.SumByProperty("name", "ben", "name", "ben2");
                Console.WriteLine("Sum count: " + sumcount);

                int result = q.GroupByProperty("name");
                Console.WriteLine("GroupbyProperty-Result: " + result);

                result = q.Transform("printName");
                Console.WriteLine("Transform-Result: " + result);

                Console.Read();

            }
        }
    }
