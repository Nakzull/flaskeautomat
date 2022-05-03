using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumer
{
    class Program
    {
        static Queue<string> products = new Queue<string>();
        static Queue<string> beer = new Queue<string>();
        static Queue<string> soda = new Queue<string>();
        static Random rng = new Random();

        static void Main(string[] args)
        {
            Thread t1 = new Thread(Produce);
            Thread t2 = new Thread(Split);

            Thread t4 = new Thread(Soda);

            t1.Name = "Producer";
            t2.Name = "Splitter";
            t4.Name = "Soda sorter";

            t1.Start();
            t2.Start();
            t4.Start();

            t1.Join();
            t2.Join();
            t4.Join();
        }

        static void Produce()
        {
            int beerCount = 1;
            int sodaCount = 1;
            while (true)
            {
                string addNext;
                lock (products)
                {
                    if (products.Count < 20)
                    {
                        int temp = rng.Next(1, 3);
                        if (temp == 1)
                        {
                            addNext = "Beer " + beerCount;
                            beerCount++;
                        }
                        else
                        {
                            addNext = "Soda " + sodaCount;
                            sodaCount++;
                        }
                        products.Enqueue(addNext);
                    }
                    else
                    {
                        Monitor.PulseAll(products);
                        Monitor.Wait(products);
                    }
                }
            }
        }
        static void Split()
        {
            while (true)
            {
                lock (products)
                {
                    while (products.Count == 0)
                    {
                        Monitor.PulseAll(products);
                        Monitor.Wait(products);
                    }
                    string type = products.Peek();
                    if (type.Substring(0,4) == "Beer")
                    {
                        if (beer.Count < 20)
                        {
                            beer.Enqueue(type);
                            Thread t3 = new Thread(Beer);
                            t3.Name = "Beer sorter";
                            t3.Start();
                            t3.Join();
                        }
                        else
                        {
                            Monitor.PulseAll(beer);
                            Monitor.Wait(beer);
                        }
                    }
                    else
                    {
                        if (soda.Count < 20)
                        {
                            soda.Enqueue(type);
                            Thread t4 = new Thread(Soda);
                            t4.Name = "Soda sorter";
                            t4.Start();
                            t4.Join();
                        }
                        else
                        {
                            Monitor.PulseAll(soda);
                            Monitor.Wait(soda);
                        }
                    }
                    products.Dequeue();
                }
            }
        }

        static void Beer()
        {
            
            lock (beer)
            {
                while (beer.Count == 0)
                {
                    Monitor.PulseAll(beer);
                    Monitor.Wait(beer);
                }
                string type = beer.Peek();
                Console.WriteLine(Thread.CurrentThread.Name + " added beer " + type.Substring(5) + " to the inventory");
                beer.Dequeue();
            }
        }

        static void Soda()
        {
            lock (soda)
            {
                while (soda.Count == 0)
                {
                    Monitor.PulseAll(soda);
                    Monitor.Wait(soda);
                }
                string type = soda.Peek();
                Console.WriteLine(Thread.CurrentThread.Name + " added soda " + type.Substring(5) + " to the inventory");
                soda.Dequeue();
            }
        }
    }
}

