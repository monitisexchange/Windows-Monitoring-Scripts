using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;

namespace TestProfilerApplication
{
    class test1
    {
        private readonly int _sleepTime;

        public test1(int sleepTime)
        {
            _sleepTime = sleepTime;
        }

        public test1()
        {
        }

        public virtual void Display()
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine("Test1");
        }

        public byte DisplayByte(string text)
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine(text);
            return 0;
        }

        public sbyte DisplaySByte(string text)
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine(text);
            return 0;
        }

        public int DisplayInt(string text)
		{
            Thread.Sleep(_sleepTime);
			Console.WriteLine(text);
			return 0;
		}

        public void ThreadPoolMethod()
        {
            ThreadPool.QueueUserWorkItem((i) =>
            {
                Console.WriteLine("threadpool");
            });
        }
    }

    class test2
    {
        private readonly int _sleepTime;

        public test2(int sleepTime)
        {
            _sleepTime = sleepTime;
        }

        public void Display()
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine("Test2");
        }

        void Display(int num)
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine(num);
        }


        public void Recursive<T>(int i)
        {
            Thread.Sleep(_sleepTime);
            Console.WriteLine("Recursive " + i);
            if (i >0)
                Recursive<T>(i - 1);
        }

        
    }

    class Program
    {
        private const int ThreadNum = 2;
        private const int RecursieveNum = 5;
        private static List<Thread> _tasks = new List<Thread>();

        static void Main(string[] args)
        {
            int sleepTime = 10;
            if (args.Length != 0)
                sleepTime = int.Parse(args[0]);
            Console.WriteLine("Test profiler");
            //test2 t2 = new test2(sleepTime);
            
           
            Thread thr1  = new Thread(ThreadProc);
            Thread thr2 = new Thread(ThreadProc);
            thr1.Start(sleepTime * 2);
            thr2.Start(sleepTime / 2);

            test1 t1 = new test1(sleepTime);
            t1.ThreadPoolMethod();
            test2 t2 = new test2(sleepTime);
            t1.Display();
            t2.Display();
            t1.DisplayByte("Byte");
            t1.DisplaySByte("SByte");
            t1.DisplayInt("Int");
            thr1.Join();
            thr2.Join();
            Thread.Sleep(5000);
            //Console.Read();
        /*   
            for (int i = 0; i < ThreadNum; i++)
            {
                var a = new Thread(() => t2.Recursive<int>(10));
                a.Start();
                _tasks.Add(a);
            }
            foreach (var s in _tasks)
            {
                s.Join();
            }*/
        }

       
        private static void ThreadProc(Object inParam)
        {
            int sleepTime = (int)inParam;
            test1 t1 = new test1(sleepTime);
            test2 t2 = new test2(sleepTime);
            t1.Display();
            t2.Display();
            t1.DisplayByte("Byte");
            t1.DisplaySByte("SByte");
            t1.DisplayInt("Int");
        }
    }

}
