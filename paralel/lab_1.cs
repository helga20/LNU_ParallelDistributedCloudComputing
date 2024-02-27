
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Potoki.Extensions;

namespace Potoki
{
    public delegate double CountDelegate (double[] n, int line_id, bool test); 
    public delegate double[] GenerateDelegate (int n, int line_id, bool test);

    enum RunType
    {
        sync=0,
        thread=1
    }

    namespace Extensions
    {
        public static class Extensions
        {
            public static void Wait(this Task[] tasks)
            {
                while (!tasks.All(task => task.IsCompleted)) { }
            }
        }
    }
    public class Config
    {
        public static readonly double  MAX_VALUE = 100;
        public static readonly double MIN_VALUE = 0;
        public static readonly int ROUND_TO = 2;   
        public static readonly int REPEAT_TIME = 10;

    }

    public class Generator
    {
        
        private static double[] GenerateLine(int size, int line_id, bool test=false)
        {
            if(!test)
                Console.WriteLine($"Generate {line_id} line, in {Thread.CurrentThread.ManagedThreadId} thread");
            var result = new double[size];
            var random = new Random();
            for (int i = 0; i < size; i++)
                result[i] = Math.Round(random.NextDouble() * (Config.MAX_VALUE - Config.MIN_VALUE) + Config.MIN_VALUE, Config.ROUND_TO);
            return result;
        }

        public static double[][] GenerateMatrixSync(int lines_count, int columns_count, bool test = false)
        {
            var result = new double[lines_count][];
            for (int i = 0; i < lines_count; i++)
                result[i] = GenerateLine(columns_count, i, test);
            return result;
        }
        public static double[][] GenerateMatrixThread(int lines_count, int columns_count, bool test = false)
        {
            var result = new double[lines_count][];
            var tasks = new Task<double[]>[lines_count];
            GenerateDelegate func = GenerateLine;
            for (int i = 0; i < lines_count; i++)
            {
                int id = i;
                tasks[i] = Task.Run(() => func(columns_count, id, test));
            }

            tasks.Wait();
            
            for (int i = 0; i < lines_count; i++)
                result[i] = tasks[i].Result;
            return result;
        }
    }

    public class SumOfElements
    {
        private static double SumInLine(double[] line, int line_id, bool test = false)
        {
            if(!test)
                Console.WriteLine($"Count sum of {line_id} line, in {Thread.CurrentThread.ManagedThreadId} thread");
            double result = 0;
            for (int i = 0; i < line.Length; i++)
                result += line[i];
            return Math.Round(result, Config.ROUND_TO);
        }
        public static double[] SumElementsInLineSync(double[][] matrix, bool test = false)
        {
            var result = new double[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
                result[i] = SumInLine(matrix[i], i, test);
            return result;
        }
        public static double[] SumElementsInLineThread(double[][] matrix, bool test = false)
        {
            var result = new double[matrix.Length];
            var tasks = new Task<double>[matrix.Length];
            CountDelegate func = SumInLine;
            for (int i = 0; i < matrix.Length; i++)
            {
                int id = i;
                tasks[i] = Task.Run(() => func(matrix[id], id, test));
            }

            tasks.Wait();
            
            for (int i = 0; i < matrix.Length; i++)
                result[i] = tasks[i].Result;
            return result;
        }
    }
    class Program
    {

        private static void ChangeThreadsCount(int n)
        {
            ThreadPool.SetMaxThreads(n, n);
            ThreadPool.SetMinThreads(n, n);
        }
        static Tuple<long, long> CompareGenerate(int n, int m)
        {
            long thread_time = 0;
            long sync_time = 0;
            for (int i = 0; i < Config.REPEAT_TIME; i++)
            {
                var local_timer = Stopwatch.StartNew();
                Generator.GenerateMatrixSync(n, m, true);
                sync_time += local_timer.ElapsedMilliseconds;
            }

            for (int i = 0; i < Config.REPEAT_TIME; i++)
            {
                var local_timer = Stopwatch.StartNew();
                Generator.GenerateMatrixThread(n, m, true);
                thread_time += local_timer.ElapsedMilliseconds;
            }
            return new Tuple<long, long>(sync_time / Config.REPEAT_TIME, thread_time / Config.REPEAT_TIME);
        }
        static Tuple<long, long> CompareSum(int n, int m)
        {
            
            long thread_time = 0;
            long sync_time = 0;
            for (int i = 0; i < Config.REPEAT_TIME; i++)
            {
                var matrix = Generator.GenerateMatrixSync(n, m, true);
                var local_timer = Stopwatch.StartNew();
                SumOfElements.SumElementsInLineSync(matrix, true);
                sync_time += local_timer.ElapsedMilliseconds;
            }

            for (int i = 0; i < Config.REPEAT_TIME; i++)
            {
                var matrix = Generator.GenerateMatrixSync(n, m, true);
                var local_timer = Stopwatch.StartNew();
                SumOfElements.SumElementsInLineThread(matrix, true);
                thread_time += local_timer.ElapsedMilliseconds;
            }
            return new Tuple<long, long>(sync_time / Config.REPEAT_TIME, thread_time / Config.REPEAT_TIME);
        }
        static double[] Run(int lines_count, int column_count, RunType type, bool test=false)
        {
            Func<int, int, bool, double[][]> generate_method = Generator.GenerateMatrixSync;
            Func<double[][], bool, double[]> count_method = SumOfElements.SumElementsInLineSync;

            if (type == RunType.thread)
            {
                generate_method = Generator.GenerateMatrixThread;
                count_method = SumOfElements.SumElementsInLineThread;
            }
            
            
            Console.WriteLine($"Run {type.ToString()} solving of problem");
            var global_timer = Stopwatch.StartNew();
            var matrix = generate_method(lines_count, column_count, test);
            if (!test)
            {
                Console.WriteLine("\nGenerated matrix:");
                foreach (var line in matrix)
                {
                    foreach (var value in line) 
                        Console.Write($"{value} ");
                    Console.WriteLine();
                }
            }
            Console.WriteLine("-----------------------------");
            var result = count_method(matrix, test);
            if (!test)
            {
                Console.WriteLine("\nResult:");
                foreach (var value in result)
                    Console.Write($"{value} ");
                
            }
            
            Console.WriteLine($"Run function end at {global_timer.ElapsedMilliseconds}");
            return result;
        }
        static void Main(string[] args)
        {
            Run(10, 10, RunType.sync);
            Console.WriteLine("===========================================");
            Run(10, 10, RunType.thread);
            Console.WriteLine("===========================================");
            
            int n = 10000;
            Console.WriteLine($"{"threads", -7}|{"lines and columns", -17}|{"sync_generate", 13}|" +
                              $"{"thread_generate", 15}|{"sync_count_result", 17}|{"thread_count_result", 19}|{"genarate_result", -15}|{"count_result", -14}");
            for (int threads = 2; threads <= 256; threads *= 2)
            {
                ChangeThreadsCount(threads);
                for (int i = 100; i <= n; i *= 10)
                {
                    for (int j = i; j <= n; j *= 10)
                    {   
                        var compare_time = CompareSum(i, j);
                        var generate_time = CompareGenerate(i, j);
            
                        double compare_result =
                            Math.Round((double)compare_time.Item2 / Math.Max(compare_time.Item1, 1), 2);
                        double genarate_result =
                            Math.Round((double)generate_time.Item2 / Math.Max(generate_time.Item1, 1), 2);
            
                        Console.WriteLine(
                            $"{threads,-7}|{$"{i}, {j}",-17}|{generate_time.Item1,-13}|{generate_time.Item2,-15}|{compare_time.Item1,-17}|{compare_time.Item2,-19}|{genarate_result,-15}|{compare_result,-14}");
                    }
                }
            }

        }
    }
}
/*
thread_count == lines
                                                RESULT(ms)
threads|lines and columns|sync_generate|thread_generate|sync_count_result|thread_count_result|genarate_result|count_result  
2      |100, 100         |0            |0              |0                |0                  |0              |0             
2      |100, 1000        |3            |0              |0                |0                  |0              |0             
2      |100, 10000       |21           |11             |2                |0                  |0.52           |0             
2      |1000, 1000       |21           |11             |2                |1                  |0.52           |0.5           
2      |1000, 10000      |185          |48             |24               |5                  |0.26           |0.21          
2      |10000, 10000     |1981         |925            |232              |60                 |0.47           |0.26          
4      |100, 100         |0            |0              |0                |0                  |0              |0             
4      |100, 1000        |2            |0              |0                |0                  |0              |0             
4      |100, 10000       |19           |7              |2                |1                  |0.37           |0.5           
4      |1000, 1000       |20           |9              |2                |0                  |0.45           |0             
4      |1000, 10000      |238          |86             |24               |7                  |0.36           |0.29          
4      |10000, 10000     |1951         |893            |233              |88                 |0.46           |0.38          
8      |100, 100         |0            |0              |0                |0                  |0              |0             
8      |100, 1000        |1            |0              |0                |0                  |0              |0             
8      |100, 10000       |19           |10             |2                |0                  |0.53           |0             
8      |1000, 1000       |19           |5              |2                |0                  |0.26           |0             
8      |1000, 10000      |189          |62             |24               |5                  |0.33           |0.21          
8      |10000, 10000     |1861         |676            |234              |53                 |0.36           |0.23          
16     |100, 100         |0            |0              |0                |0                  |0              |0             
16     |100, 1000        |2            |0              |0                |0                  |0              |0             
16     |100, 10000       |16           |6              |2                |0                  |0.38           |0             
16     |1000, 1000       |17           |7              |2                |0                  |0.41           |0             
16     |1000, 10000      |187          |68             |23               |5                  |0.36           |0.22          
16     |10000, 10000     |1883         |956            |234              |52                 |0.51           |0.22          
32     |100, 100         |0            |0              |0                |0                  |0              |0             
32     |100, 1000        |2            |0              |0                |0                  |0              |0             
32     |100, 10000       |19           |45             |2                |0                  |2.37           |0             
32     |1000, 1000       |16           |16             |2                |0                  |1              |0             
32     |1000, 10000      |180          |110            |23               |5                  |0.61           |0.22          
32     |10000, 10000     |1970         |3670           |233              |51                 |1.86           |0.22          
64     |100, 100         |0            |0              |0                |0                  |0              |0             
64     |100, 1000        |2            |7              |0                |0                  |3.5            |0             
64     |100, 10000       |18           |101            |2                |0                  |5.61           |0             
64     |1000, 1000       |19           |107            |2                |0                  |5.63           |0             
64     |1000, 10000      |192          |190            |24               |5                  |0.99           |0.21          
64     |10000, 10000     |1908         |5824           |233              |52                 |3.05           |0.22          
128    |100, 100         |0            |0              |0                |1                  |0              |1             
128    |100, 1000        |1            |1              |0                |0                  |1              |0             
128    |100, 10000       |18           |191            |2                |0                  |10.61          |0             
128    |1000, 1000       |17           |11             |2                |5                  |0.65           |2.5           
128    |1000, 10000      |252          |826            |23               |5                  |3.28           |0.22          
128    |10000, 10000     |1980         |15607          |232              |50                 |7.88           |0.22          
256    |100, 100         |0            |0              |0                |0                  |0              |0             
256    |100, 1000        |2            |5              |0                |0                  |2.5            |0             
256    |100, 10000       |18           |11             |2                |0                  |0.61           |0             
256    |1000, 1000       |19           |210            |2                |1                  |11.05          |0.5           
256    |1000, 10000      |198          |2942           |23               |6                  |14.86          |0.26          
256    |10000, 10000     |1968         |39119          |234              |53                 |19.88          |0.23
*/
