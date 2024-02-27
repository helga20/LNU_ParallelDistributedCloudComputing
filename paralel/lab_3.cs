
using System.Collections.Concurrent;
using System.Diagnostics;
using Accord.Math;

namespace Potoki
{
    public class Config
    {
        public static readonly double  MAX_VALUE = 100;
        public static readonly double MIN_VALUE = 0;
        public static readonly int ROUND_TO = 2;
        public static readonly int REPEAT = 10;
        public static readonly int UPPER_BOUND = 1000;
    }
    static class DoubleExtensions
    {
        public static double Round(this double num, int k = 2)
        {
            return Math.Round(num, Config.ROUND_TO);
        }
    }
    static class Extensions
    {
        public static double Determinant(this double[,] matrix, int thread_count = 1, int start_row=0, ConcurrentDictionary<int, bool>? ignored_columns=null)
        {
            if (ignored_columns == null)
                ignored_columns = new();
            if (start_row >= matrix.GetLength(0) - 1 || matrix.GetLength(0) - ignored_columns.Count < 2)
                return 0;
            if (matrix.GetLength(0) - ignored_columns.Count == 2)
            {
                int[] columns = new int[2];
                int j = 0;
                for(int i = 0; i < matrix.GetLength(0); i++)
                    if (!ignored_columns.ContainsKey(i))
                    {
                        columns[j++] = i;
                        if(j == 2)
                            break;
                    }
                return matrix[start_row, columns[0]] * matrix[start_row + 1, columns[1]] - matrix[start_row + 1, columns[0]] * matrix[0, columns[1]];
            }            
            
            int per_thread = (int)Math.Max(1, Math.Ceiling((double)(matrix.GetLength(0) / thread_count)));

            double result = 0;
            int counter = 0;
            List<Thread> threads = new();
            for (int start_id = 0; start_id < matrix.GetLength(0); start_id += per_thread)
            {
                var left_bound = start_id;
                var right_bound = Math.Min(start_id + per_thread, matrix.GetLength(0));
                threads.Add(new Thread(() =>
                {
                    for (int i = left_bound; i < right_bound; i++)
                    {
                        if (ignored_columns.ContainsKey(i))
                            continue;
                        var new_ignored = ignored_columns;
                        new_ignored[i] = true;
                        result += matrix.Determinant(1, start_row + 1, new_ignored) * 
                                  matrix[start_row, i] * (counter % 2 != 0 ? -1 : 1);
                        counter += 1;
                    }
                }));
                threads.Last().Start();
            }

            foreach (var thread in threads)
                thread.Join();
            
            return result;
        }
        public static double Multiply(this double[] vector_a, double[] vector_b)
        {
            double result = 0;
            for (int i = 0; i < vector_a.Length; i++)
                result += (vector_a[i] * vector_b[i]).Round();
            return result;
        }
        private static double[] MultiplySync(this double[][] matrix, double[] vector, int left_bound=0, int? right_bound=null, bool test = false)
        {
            if (right_bound == null || right_bound > matrix.GetLength(0))
                right_bound = matrix.GetLength(0);
            if(!test)
                Console.WriteLine($"Multiply [{left_bound}, {right_bound}) lines, in {Thread.CurrentThread.ManagedThreadId} thread");
            var result = new double[(int)right_bound - left_bound];
            for (int i = 0; i < result.Length; i++)
                result[i] = matrix[i].Multiply(vector);
            return result;
        }
        public static double[] Multiply(this double[][] matrix, double[] vector, int thread_count = 1, bool test = false)
        {
            var result = new double[vector.Length];
            List<Thread> threads = new();
            double lines_count = matrix.GetLength(0);
            int per_thread = (int)Math.Max(1, Math.Ceiling((lines_count / thread_count)));
            var n = matrix.GetLength(0);
            for (int i = 0; i < n; i += per_thread)
            {
                var left_bound = i;
                var right_bound = Math.Min(i + per_thread, matrix.GetLength(0));

                threads.Add(new Thread(() =>
                {
                    var calculation_result = matrix.MultiplySync(vector, left_bound, right_bound, test);
                    foreach (var val in calculation_result)
                        result[left_bound++] = val;
                }));
                threads.Last().Start();
            }
            foreach (var thread in threads)
                thread.Join();
            return result;
        }

    }
    class MATH
    {
        public static double[] matrix_method(double[,] matrix, double[] result_vector, int thread_count = 1, bool test=false)
        {
            var determ = matrix.Determinant(thread_count);
            if (determ == 0)
            {
                if (test)
                    return result_vector;
                return result_vector;
                throw new Exception("determinant is zero");
            }
            var matrix_inverse = matrix.Inverse();
            var array_matrix = matrix_inverse.ToJagged();

            return array_matrix.Multiply(result_vector, thread_count, test);
        }
    }

    class Generate
    {
        public static double[,] Matrix(int n)
        {
            var result = new double[n, n];
            var random = new Random();
            for(int i = 0; i < n; i++)
                for(int j = 0; j < n; j++)
                    result[i, j] = (random.NextDouble() * (Config.MAX_VALUE - Config.MIN_VALUE) + Config.MIN_VALUE).Round();
            return result;
        }

        public static double[] Vector(int n)
        {
            var result = new double[n];
            var random = new Random();
            for(int i = 0; i < n; i++)
                result[i] = (random.NextDouble() * (Config.MAX_VALUE - Config.MIN_VALUE) + Config.MIN_VALUE).Round();
            return result;
        }
    }

    class Compare
    {
        public static long GetTime(int n, int threads_count=1)
        {
            long result = 0;
            for (int i = 0; i < Config.REPEAT; i++)
            {
                var A = Generate.Matrix(n);
                var B = Generate.Vector(n);
                var timer = Stopwatch.StartNew();
                MATH.matrix_method(A, B, threads_count, true);
                result += timer.ElapsedMilliseconds;
            }

            return result / Config.REPEAT;
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            double[,] A = { { 1, -1, 1 }, { 3, 2, -1 }, { 2, -3, 3 } };
            double[] b = { 1, 6, 0 };
            var thread_result = MATH.matrix_method(A, b, 2);
            var sync_result = MATH.matrix_method(A, b, 1);
            MATH.matrix_method(Generate.Matrix(10), Generate.Vector(10), 1);
            foreach (var val in thread_result)
                Console.Write($"{val}; ");
            Console.Write("== ");
            foreach (var val in sync_result)
                Console.Write($"{val}; ");
            Console.WriteLine($"== 3; -5; -7");
            List<long> sync_times = new();
            for (int i = 10; i <= Config.UPPER_BOUND; i *= 10)
            {
                Console.WriteLine($"start for {i}");
                sync_times.Add(Compare.GetTime(i));
            }
            
            Console.WriteLine("====================================================================");
            Console.WriteLine($"{"threads", -7}|{"size", -5}|{"sync time", -9}|{"thread time", -11}|{"acceleration", -13}");

            for (int threads = 2; threads <= 256; threads *= 2)
            {
                int j = 0;
                for (int i = 10; i <= Config.UPPER_BOUND; i *= 10)
                {
                    long thread_time = Compare.GetTime(i, threads);
                    long sync_time = sync_times[j++];
                    var acceleration = ((double)sync_time / Math.Max(1, thread_time)).Round();
                    Console.WriteLine($"{threads, -7}|{i, -5}|{sync_time, -9}|{thread_time, -11}|{acceleration, -13}");
                }
            }
        }
    }
    /*
        ====================================================================
        threads|size |sync time|thread time|acceleration 
        2      |10   |0        |0          |0            
        2      |100  |14       |7          |2            
        2      |1000 |3363     |1377       |2.44         
        4      |10   |0        |0          |0            
        4      |100  |14       |7          |2            
        4      |1000 |3363     |735        |4.58         
        8      |10   |0        |0          |0            
        8      |100  |14       |14         |1            
        8      |1000 |3363     |785        |4.28         
        16     |10   |0        |0          |0            
        16     |100  |14       |10         |1.4          
        16     |1000 |3363     |267        |12.6         
        32     |10   |0        |2          |0            
        32     |100  |14       |55         |0.25         
        32     |1000 |3363     |530        |6.35         
        64     |10   |0        |1          |0            
        64     |100  |14       |15         |0.93         
        64     |1000 |3363     |621        |5.42         
        128    |10   |0        |0          |0            
        128    |100  |14       |15         |0.93         
        128    |1000 |3363     |396        |8.49         
        256    |10   |0        |0          |0            
        256    |100  |14       |33         |0.42         
        256    |1000 |3363     |206        |16.33 
     */
}

