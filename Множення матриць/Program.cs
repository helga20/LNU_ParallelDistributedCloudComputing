using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Parallel2
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

        public static int[,] generateMatrix(int n, int m)
        {
            int[,] matrix = new int[n, m];
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = rnd.Next(0, 10);
                }
            }
            return matrix;
        }
        public static TimeSpan SyncMethod(int[,] matrixA, int[,] matrixB)
        {
            int rows1 = matrixA.GetLength(0);
            int cols1 = matrixA.GetLength(1);
            int rows2 = matrixB.GetLength(0);
            int cols2 = matrixB.GetLength(1);
            if (cols1 != rows2)
            {
                throw new ArgumentException("Matrices cannot be multiplied.");
            }
            int[,] result = new int[rows1, cols2];

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < rows1; i++)
            {
                for (int j = 0; j < cols2; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < cols1; k++)
                    {
                        sum += matrixA[i, k] * matrixB[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Sync time ~ " + stopWatch.Elapsed.ToString());
            return stopWatch.Elapsed;
        }

        public static TimeSpan AsyncMethod(int[,] matrixA, int[,] matrixB, int threadNum)
        {
            int rowsA = matrixA.GetLength(0);
            int colsA = matrixA.GetLength(1);
            int rowsB = matrixB.GetLength(0);
            int colsB = matrixB.GetLength(1);

            if (colsA != rowsB)
            {
                throw new ArgumentException("Matrices cannot be multiplied.");
            }

            int[,] result = new int[rowsA, colsB];

            int totalElements = rowsA * colsB;
            int elementsPerThread = totalElements / threadNum;

            List<Thread> threads = new List<Thread>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int start = 0;
            for (int i = 0; i < threadNum; i++)
            {
                int end = (i == threadNum - 1) ? totalElements : start + elementsPerThread;

                Thread thread = new Thread((object range) =>
                {
                    int[] rangeArray = (int[])range;
                    int startRange = rangeArray[0];
                    int endRange = rangeArray[1];

                    for (int index = startRange; index < endRange; index++)
                    {
                        int row = index / colsB;
                        int col = index % colsB;

                        int sum = 0;
                        for (int k = 0; k < colsA; k++)
                        {
                            sum += matrixA[row, k] * matrixB[k, col];
                        }
                        result[row, col] = sum;
                    }
                });

                threads.Add(thread);
                thread.Start(new int[] { start, end });

                start = end;
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
            int n = 2000;
            int m = 300;
            int t = 8;

            var m1 = generateMatrix(n, m);
            var m2 = generateMatrix(m, n);

            Console.WriteLine($"[{n}][{m}] * [{m}][{n}]");

            TimeSpan sync = SyncMethod(m1, m2);
            TimeSpan async = AsyncMethod(m1, m2, t);

            var acceleration = sync / async;
            var efficiency = acceleration / t;

            Console.WriteLine($"Acceleration ~ {acceleration}");
            Console.WriteLine($"Efficiency ~ {efficiency}");
        }
    }

}