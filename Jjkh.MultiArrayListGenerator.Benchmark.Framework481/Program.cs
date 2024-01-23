using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Jjkh.Generators;

namespace Framework481
{
    internal partial class Program
    {
        [MultiArrayList]
        struct TestData
        {
            public float X;
            public float Y;
            public bool Deleted;
            public DateTime Created;

            public static List<TestData> GenerateRandom(int count)
            {
                var rand = new Random();

                var midInt = int.MaxValue / 2;
                var now = DateTime.Now;

                var data = new List<TestData>(count);
                for (int i = 0; i < count; i++)
                {
                    data.Add(new TestData
                    {
                        X = (float)rand.NextDouble(),
                        Y = (float)rand.NextDouble(),
                        Deleted = rand.Next() > midInt,
                        Created = now
                    });
                }
                return data;
            }
        }

        static void Main()
        {
            var sw = new Stopwatch();

            sw.Start();
            var soaData = TestData.GenerateRandom(100_000_000);
            Console.WriteLine($"Generated data in {sw.Elapsed}");

            sw.Restart();
            Console.WriteLine($"Average X: {soaData.Sum(d => d.X) / soaData.Count}");
            Console.WriteLine($"Average Y: {soaData.Sum(d => d.Y) / soaData.Count}");
            Console.WriteLine($"Deleted: {soaData.Count(d => d.Deleted)}");
            var aosSpeed = sw.Elapsed;
            Console.WriteLine($"^^^ took {aosSpeed}");

            sw.Restart();
            var aosData = new TestDataMultiArray(soaData);
            Console.WriteLine($"Converted to MultiArrayList in {sw.Elapsed}");

            sw.Restart();
            Console.WriteLine($"Average X: {aosData.X.Sum() / aosData.Count}");
            Console.WriteLine($"Average Y: {aosData.Y.Sum() / aosData.Count}");
            Console.WriteLine($"Deleted: {aosData.Deleted.Count(x => x)}");
            var soaSpeed = sw.Elapsed;
            Console.WriteLine($"^^^ took {sw.Elapsed} ({aosSpeed.TotalMilliseconds / soaSpeed.TotalMilliseconds:F2}x speedup)");

            Console.ReadLine();
        }
    }
}
