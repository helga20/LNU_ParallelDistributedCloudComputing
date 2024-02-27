using System.Diagnostics;
using Accord.Math;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using LibVector = MathNet.Numerics.LinearAlgebra.Vector<double>; 

namespace Potoki
{
    public class Config
    {
        public static readonly double  MAX_VALUE = 100;
        public static readonly double MIN_VALUE = 0;
        public static readonly int ROUND_TO = 4;
        public static readonly int ITERATION_COUNT = 10;
        public static readonly double EPSILON = 0.00001;
        public static readonly int REPEAT = 3;
        public static readonly int UPPER_BOUND = 10000;
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
        public static double Multiply(this double[] vector_a, LibVector vector_b)
        {
            double result = 0;
            for (int i = 0; i < vector_a.Length; i++)
                result += (vector_a[i] * vector_b[i]).Round();
            return result;
        }
        public static LibVector Multiply(this double[][] matrix, LibVector vector)
        {
            var result = new DenseVector(vector.Count);
            var is_completed = new bool[vector.Count];
            var n = matrix.GetLength(0);
            for (int i = 0; i < n; i++)
                ThreadPool.QueueUserWorkItem((id) =>
                {
                    int i = (int)id;
                    result[i] = matrix.GetRow(i).Multiply(vector);
                    is_completed[i] = true;
                }, i);
            
            while (!is_completed.All(obj => obj));
            return result;
        }

    }
    class MATH
    {
        private static (LibVector h, LibVector q) MakeArnoldiIteration(double[][] A, Matrix<double> Q, int k)
        {
            var h = new DenseVector(k + 1);
            var is_completed = new bool[k];
            var q = A.Multiply(Q.Column(k - 1));
            
            for (int i = 0; i < k; i++)
                ThreadPool.QueueUserWorkItem((id) =>
                {
                    int i = (int)id;
                    
                    var q_i = Q.Column(i);
                    h[i] = q * q_i;
                    q.Subtract(h[i] * q_i, q);
                    
                    is_completed[i] = true;
                }, i);
            
            while (!is_completed.All(obj => obj));
            // for (int i = 0; i < k; i++)
            // {
            //     var q_i = Q.Column(i);
            //     h[i] = q * q_i;
            //     q.Subtract(h[i] * q_i, q); 
            // }

            var q_norm = q.Norm(2);
            h[k] = q_norm;
            q /= q_norm;

            return (h, q);
        }

        public static (double sin, double cos) CreateRotation(double x, double y)
        {
            if (Math.Abs(x) < double.Epsilon)
                return (0.0, 1.0);

            var t = Math.Sqrt(x * x + y * y);
            var cos = Math.Abs(x) / t;
            var sin = cos * y / x;
            return (sin, cos);
        }

        private static (LibVector h, double sin, double cos) ApplyGivenRotationsToHColumn(LibVector h, double[] sin, double[] cos, int k)
        {
            for (var i = 0; i < k; i++)
            {
                var temp = cos[i] * h[i] + sin[i] * h[i + 1];
                h[i + 1] = -sin[i] * h[i] + cos[i] * h[i + 1];
                h[i] = temp;
            }
            (var sin_k, var cos_k) = CreateRotation(h[k], h[k + 1]);

            h[k] = cos_k * h[k] + sin_k * h[k + 1];
            h[k + 1] = 0.0;

            return (h, sin_k, cos_k);
        }
        public static LibVector gmres(double[][] A, LibVector b)
        {
            var x0 = new DenseVector(b.Count);
            
            var n = A.GetLength(0);
            var m = Config.ITERATION_COUNT;
            
            var r0 = b - A.Multiply(x0);
            var b_norm = b.Norm(2);
            
            var rotations_sin = new double[m];
            var rotations_cos = new double[m];
            
            var r0_norm = r0.Norm(2);
            if (r0_norm / b_norm <= Config.EPSILON)
                return x0.Clone();
            
            var Q = new DenseMatrix(n, m + 1);
            Q.SetColumn(0, r0 / r0_norm);
            
            var beta = new DenseVector(n + 1) {[0] = r0_norm}; 
            var H = new DenseMatrix(m + 1, m);
            
            int i;
            for (i = 0; i < m; i++)
            {
                var (h_column, q) = MakeArnoldiIteration(A, Q, i + 1);
                Q.SetColumn(i + 1, 0, q.Count, q);

                (var rotated_column, rotations_sin[i], rotations_cos[i]) =
                    ApplyGivenRotationsToHColumn(h_column, rotations_sin, rotations_cos, i);

                H.SetColumn(i, 0, rotated_column.Count, rotated_column);
                beta[i + 1] = -rotations_sin[i] * beta[i];
                beta[i] = rotations_cos[i] * beta[i];
                var error = Math.Abs(beta[i + 1]) / b_norm;

                if (error <= Config.EPSILON)
                    break;
            }
        
            var y = H.SubMatrix(0, i, 0, i).Solve(beta.SubVector(0, i));
            var x = Q.SubMatrix(0, Q.RowCount, 0, i) * y;
            for (i = 0; i < x.Count; i++)
                x[i] = x[i].Round();
            return x;
        }

    }

    class Generate
    {
        public static double[][] Matrix(int n)
        {
            var result = new double[n][];
            var random = new Random();
            for (int i = 0; i < n; i++)
            {
                result[i] = new double[n];
                for (int j = 0; j < n; j++)
                    result[i][j] = (random.NextDouble() * (Config.MAX_VALUE - Config.MIN_VALUE) + Config.MIN_VALUE).Round();
            }

            return result;
        }

        public static LibVector Vector(int n)
        {
            var result = new DenseVector(n);
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
                MATH.gmres(A, B);
                result += timer.ElapsedMilliseconds;
            }

            return result / Config.REPEAT;
        }
    }
    class Program
    {
        public static void SetThreadCount(int n)
        {
            ThreadPool.SetMaxThreads(n, n);
            ThreadPool.SetMinThreads(n, n);
        }
        public static void Main(string[] args)
        {
            var A = new double[][]
            {
                new double[]{ 10, 0, 0, 0, 0, 0 }, 
                new double[]{ 0, 10, -3, -1, 0, 0 }, 
                new double[]{ 0, 0, 15, 0, 0, 0 }, 
                new double[]{ -2, 0, 0, 10, -1, 0 }, 
                new double[]{ -1, 0, 0, -5, 1, -3 }, 
                new double[]{ -1, -2, 0, 0, 0, 6 }, 
            };
            LibVector b = new DenseVector(new double[]{10, 7, 45, 33, -34, 31 });
            var thread_result = MATH.gmres(A, b);
            foreach (var val in thread_result)
                Console.Write($"{val}; ");
            Console.Write("== ");
            var sync_result = MATH.gmres(A, b);
            SetThreadCount(1);
            foreach (var val in sync_result)
                Console.Write($"{val}; "); 
            Console.WriteLine("== 1; 2; 3; 4; 5; 6");
           
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
                SetThreadCount(threads);
                int j = 0;
                for (int i = 10; i <= Config.UPPER_BOUND; i *= 10)
                {
                    var sync_time = sync_times[j++];
                    long thread_time = Compare.GetTime(i, threads);
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
        2      |100  |3        |3          |1            
        2      |1000 |103      |194        |0.5309       
        2      |10000|6794     |19204      |0.3538       
        4      |10   |0        |0          |0            
        4      |100  |3        |1          |3            
        4      |1000 |103      |98         |1.051        
        4      |10000|6794     |8140       |0.8346       
        8      |10   |0        |0          |0            
        8      |100  |3        |0          |3            
        8      |1000 |103      |59         |1.7458       
        8      |10000|6794     |5616       |1.2098       
        16     |10   |0        |1          |0            
        16     |100  |3        |1          |3            
        16     |1000 |103      |66         |1.5606       
        16     |10000|6794     |5795       |1.1724       
        32     |10   |0        |0          |0            
        32     |100  |3        |3          |1            
        32     |1000 |103      |100        |1.03         
        32     |10000|6794     |6956       |0.9767       
        64     |10   |0        |0          |0            
        64     |100  |3        |3          |1            
        64     |1000 |103      |186        |0.5538       
        64     |10000|6794     |14114      |0.4814       
        128    |10   |0        |0          |0            
        128    |100  |3        |4          |0.75         
        128    |1000 |103      |687        |0.1499       
        128    |10000|6794     |76103      |0.0893       
        256    |10   |0        |0          |0            
        256    |100  |3        |1          |3            
        256    |1000 |103      |688        |0.1497
     */
}


