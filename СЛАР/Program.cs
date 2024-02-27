using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace Parallel2
{
    internal class Program
    {
        public static void Print(double[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                Console.Write("|");
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    Console.Write(m[i, j] + " ");
                    if (j == m.GetLength(1) - 2) Console.Write("| ");
                }
                Console.Write("|\n");
            }
            Console.WriteLine();
        }

        public static double[,] generateMatrix(int n, int m)
        {
            double[,] matrix = new double[n, m];
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

        public static void CheckZerosOnDiagonal(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, i] == 0) throw new DivideByZeroException();
            }
        }


        public static TimeSpan SyncMethod(double[,] matrix)
        {
            int N = matrix.GetLength(0);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Зводимо матрицю до верхньої трикутної
            for (int rowNum = 0; rowNum < N; rowNum++)
            {
                for (int i = rowNum + 1; i < N; i++)
                {
                    double factor = matrix[i, rowNum] / matrix[rowNum, rowNum];
                    for (int j = rowNum; j <= N; j++)
                    {
                        matrix[i, j] -= factor * matrix[rowNum, j];
                    }
                }
            }
            CheckZerosOnDiagonal(matrix);

            double[] result = new double[N];

            // Обернений хід методу Жордана-Гаусса для знаходження розв'язку
            for (int rowNum = N - 1; rowNum >= 0; rowNum--)
            {
                result[rowNum] = matrix[rowNum, N];
                for (int j = rowNum + 1; j < N; j++)
                {
                    result[rowNum] -= matrix[rowNum, j] * result[j];
                }
                result[rowNum] /= matrix[rowNum, rowNum];
            }

            stopWatch.Stop();

            Console.WriteLine("Sync time ~ " + stopWatch.Elapsed.ToString());
            return stopWatch.Elapsed;
        }

        public static TimeSpan AsyncMethod(double[,] matrix, int numThreads)
        {
            int N = matrix.GetLength(0);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<Thread> threads = new List<Thread>();
            int rowsPerThread = N / numThreads;

            for (int i = 0; i < numThreads; i++)
            {
                int startRow = i * rowsPerThread;
                int endRow = (i == numThreads - 1) ? N : startRow + rowsPerThread;

                Thread thread = new Thread((object range) =>
                {
                    int[] rangeArray = (int[])range;
                    int startRange = rangeArray[0];
                    int endRange = rangeArray[1];

                    for (int rowNum = startRange; rowNum < endRange; rowNum++)
                    {
                        for (int i = rowNum + 1; i < N; i++)
                        {
                            double factor = matrix[i, rowNum] / matrix[rowNum, rowNum];
                            for (int j = rowNum; j <= N; j++)
                            {
                                matrix[i, j] -= factor * matrix[rowNum, j];
                            }
                        }
                    }
                });

                threads.Add(thread);
                thread.Start(new int[] { startRow, endRow });
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }

            CheckZerosOnDiagonal(matrix);

            // Отримуємо результат (на жаль вже одним потоком)
            double[] result = new double[N];
            for (int rowNum = N - 1; rowNum >= 0; rowNum--)
            {
                result[rowNum] = matrix[rowNum, N];
                for (int j = rowNum + 1; j < N; j++)
                {
                    result[rowNum] -= matrix[rowNum, j] * result[j];
                }
                result[rowNum] /= matrix[rowNum, rowNum];
            }
            stopWatch.Stop();

            Console.WriteLine($"Async time ~ {stopWatch.Elapsed} with {numThreads} threads");
            return stopWatch.Elapsed;
        }


        static void Main(string[] args)
        {
            int n = 2000;
            int t = 20;

            double[,] matrix = generateMatrix(n, n + 1);

            Console.WriteLine($"N = {n}");

            try
            {

                TimeSpan sync = SyncMethod(matrix);
                TimeSpan async = AsyncMethod(matrix, t);

                var acceleration = sync / async;
                var efficiency = acceleration / t;

                Console.WriteLine($"Acceleration ~ {acceleration}");
                Console.WriteLine($"Efficiency ~ {efficiency}");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Attempt to divide by zero.");
            }
        }
    }

}