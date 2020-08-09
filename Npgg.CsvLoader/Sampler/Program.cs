using System;
using System.Diagnostics;
using Npgg.CsvLoader;

namespace Sampler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Test1();
        }


        static void Test1()
        {
            var type = typeof(Sample1);
            
            foreach(var member in type.GetMember("Value1"))
            {
                int count = 5000000;
                var obj = new Sample1();

                var es1 = Run(count, new MemberAssigner(member), (gs, i) =>
                {
                    gs.SetValue(obj, i.ToString());
                    var ret = gs.GetValue(obj);
                    
                    if((int)ret != i)
                    {

                    }
                  
                });

      
            }
            
            
        }

        static long Run<T>(int loop, T source, Action<T, int> action)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            for (int i = 0; i < loop;i++)
            {
                action(source, i);
            }
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }
    }


    class Sample1
    {
        public int Value1 { get; set; }
        //public int Value2 { get; set; }
        //public int Value3 { get; set; }
    }
}
