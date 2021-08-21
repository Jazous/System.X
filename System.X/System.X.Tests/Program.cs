using System;
using System.Collections.Generic;

namespace System.X.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            return;
            //0、1、1 - 1、2批次、10、10批次
            List<string> dataList = new List<string>() { "c", "0", "1", "e", "1-1", "10", "2", "2批次", "1批次", "1 c", "a", "1-大", "11", "11 批次", "11-01", "24", "10批次", "d", "a2", " " };
            //List<string> dataList = new List<string>() { "1", "1-1", "2", "2批次", "1批次", "1 c", " " };
            var watch = new Diagnostics.Stopwatch();
            watch.Start();
            //dataList.Sort(new StringIntegerComparer());
            watch.Stop();
            //foreach (string data in dataList)
            //{
            //    Console.WriteLine(data);
            //}
            //Console.WriteLine(watch.ElapsedTicks);
        }
    }
}