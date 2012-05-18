using System;
using System.Collections.Generic;
using System.Text;

namespace TestProfilerApplication
{
    class test1
    {
    public
        virtual void Display()
        {
            Console.WriteLine("Test1");
        }
    }

    class test2
    {
    public
        void Display()
        {
            Console.WriteLine("Test2");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test profiler");
            test1 t1 = new test1();
            test2 t2 = new test2();
            t1.Display();
            t2.Display();
        }
    }

}
