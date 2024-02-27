
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace paralel1
{
    internal class Program
    {
        public static void Print(int[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    Console.Write(m[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static int sizeInput()
        {
            int size;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out size) && size > 0)
                {
                    return size;
                }
                Console.WriteLine("Wrong input");
            }
        }
        public static int[,] generateMatrix(int n, int m)
        {
            int[,] matrix = new int[n, m];
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = rnd.Next(0, 100);
                }
            }
            return matrix;
        }
        public static TimeSpan SyncMethod(int[,] matrixA, int[,] matrixB)
        {
            int n = matrixA.GetLength(0);
            int m = matrixA.GetLength(1);
            int[,] result = new int[n, m];

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = matrixA[i, j] + matrixB[i, j];
                }
            }
            stopWatch.Stop();

            Console.WriteLine("Sync time ~ " + stopWatch.Elapsed.ToString());
            return stopWatch.Elapsed;
        }

        public static TimeSpan AsyncMethod(int[,] matrixA, int[,] matrixB, int threadNum)
        {

            int n = matrixA.GetLength(0);
            int m = matrixA.GetLength(1);
            int[,] result = new int[n, m];

            void ComputeSubmatrix(int startRow, int endRow)
            {
                for (int i = startRow; i < endRow; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        result[i, j] = matrixA[i, j] + matrixB[i, j];
                    }
                }
            }

            Thread[] threads = new Thread[threadNum];
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int rowsPerThread = n / threadNum;
            for (int i = 0; i < threadNum; i++)
            {
                int startRow = i * rowsPerThread;
                int endRow = (i == threadNum - 1) ? n : (i + 1) * rowsPerThread;

                threads[i] = new Thread(() => ComputeSubmatrix(startRow, endRow));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            stopWatch.Stop();
            Console.WriteLine($"Async time ~ {stopWatch.Elapsed} with {threadNum} threads");
            return stopWatch.Elapsed;
        }
        static void Main(string[] args)
        {
            int n = 20000;
            int m = 30000;
            int t = 19;

            var m1 = generateMatrix(n, m);
            var m2 = generateMatrix(n, m);

            Console.WriteLine($"N = {n}  M = {m}");

            TimeSpan sync = SyncMethod(m1, m2);
            TimeSpan async = AsyncMethod(m1, m2, t);

            var acceleration = sync / async;
            var efficiency = acceleration / t;

            Console.WriteLine($"Acceleration ~ {acceleration}");
            Console.WriteLine($"Efficiency ~ {efficiency}");
        }
    }
}