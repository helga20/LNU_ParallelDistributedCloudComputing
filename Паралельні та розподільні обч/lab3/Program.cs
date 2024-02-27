using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace lab3
{
    internal static class Program
    {
        public static void GaussMethod(double[,] A, double[] B)
        {
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            double[] X = new double[A.GetLength(0)];
            int Size = A.GetLength(1);
            for (int j = 0; j < Size - 1; j++)
            {
                var max = Math.Abs(A[j, j]);
                int maxLine = j;
                for (int i = j + 1; i < Size; i++)
                {
                    if (Math.Abs(A[i, j]) > max)
                    {
                        max = Math.Abs(A[i, j]);
                        maxLine = i;
                    }
                }
                        
                if (maxLine != j)
                {
                    for (int k = j; k < Size; k++)
                    {
                        (A[j, k], A[maxLine, k]) = (A[maxLine, k], A[j, k]);
                    }
                    (B[j], B[maxLine]) = (B[maxLine], B[j]);
                }
                        
                for (int i = j + 1; i < Size; i++)
                {
                    double mult = A[i, j] / max;
                    for (int k = j; k < Size; k++)
                    {
                        A[i, k] -= mult * A[j, k];
                    }
                    B[i] -= mult * B[j];
                }
            }
            for (int j = Size - 1; j >= 0; j--)
            {
                if (A[j, j] == 0)
                {
                    Console.WriteLine("Wrong input");
                    return;
                }
                X[j] = B[j] / A[j, j];
                for (int i = 0; i < j; i++)
                {
                    B[i] -= A[i, j] * X[j];
                }
            }

            if (A.GetLength(0) == 2)
            {
                Console.WriteLine("Result in gausse methot = : ");
                foreach (var x in X)
                {
                    Console.Write(x + "  ");
                }
            }
            
            sw.Stop();
            Console.WriteLine("Time for Gausse method = {0}", sw.ElapsedMilliseconds);
        }
        
        public static void GaussWithThreads(double[,] A, double[] B,  int threadCount)
        {
            double[] X = new double[A.GetLength(0)];
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            int Size = A.GetLength(0);
            Thread[] threads = new Thread[threadCount];
            for (int j = 0; j < Size - 1; j++)
            {
                double max = Math.Abs(A[j, j]);
                int maxLine = j;
                for (int i = j + 1; i < Size; i++)
                {
                    if (Math.Abs(A[i, j]) > max)
                    {
                        max = Math.Abs(A[i, j]);
                        maxLine = i;
                    }
                }
                if (maxLine != j)
                {
                    for (int k = j; k < Size; k++)
                    {
                        (A[j, k], A[maxLine, k]) = (A[maxLine, k], A[j, k]);
                    }
                    (B[j], B[maxLine]) = (B[maxLine], B[j]);
                }

                for (int index = 0; index < threadCount; index++)
                {
                    threads[index] = new Thread(() => SubtractionFromMatrix(A, Size, B, index, j, max));
                }
                foreach (var t in threads)
                {
                    t.Start();
                }

                foreach (var t in threads)
                {
                    t.Join();
                }
            }
            for (int j = Size - 1; j >= 0; j--)
            {
                if (A[j, j] == 0)
                {
                    Console.WriteLine("Wrong input");
                    return;
                }
                X[j] = B[j] / A[j, j];
                for (int index = 0; index < threadCount; index++)
                {
                    threads[index] = new Thread(() => GetResult(A, Size, B, X, index, j));
                }
                foreach (var t in threads)
                {
                    t.Start();
                }

                foreach (var t in threads)
                {
                    t.Join();
                }

            }
            if (A.GetLength(0) == 2)
            {
                Console.WriteLine("Result in gausse methot with thread = : ");
                foreach (var x in X)
                {
                    Console.Write(x + "  ");
                }
            }
            sw.Stop();
            Console.WriteLine("Time for Gausse method with threads = {0}", sw.ElapsedMilliseconds);
        }
        public static void GetResult(double[,] A, int Size, double[] B, double[] X, int index, int j)
        {
            for (int i = index; i < j; i += 4)
            {
                B[i] -= A[i, j] * X[j];
            }
        }
        public static void SubtractionFromMatrix(double[,] A, int Size, double[] B, int index, int j, double max)
        {
            for (int i = j + 1 + index; i < Size; i += 5)
            {
                double mult = A[i, j] / max;
                for (int k = j; k < Size; k++)
                {
                    A[i, k] -= mult * A[j, k];
                }
                B[i] -= mult * B[j];
            }
            //Console.WriteLine();
            //Matrix.ShowMatrix(A, Size, B);
            //Console.WriteLine();
        }


        static void GradientMethod(double[,] A, double[,] b)
        {
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            double[,] x = new double[b.GetLength(0), b.GetLength(1)];
            
            double[,] r = MinusMatrix(b, MultiplyMatrix(A, x));
            
            double[,] p = r;
            int k = 0;
            double alpha;
            double beta;
            
            while (k != A.GetLength(1))
            {
                
                alpha = (MultiplyMatrix(trans_matrix(r), r)[0, 0]) /
                        (MultiplyMatrix(MultiplyMatrix(trans_matrix(p), A), p)[0, 0]);
                
                x = AddMatrix(x, MultiplyMatrixByNumber(p, alpha));
                var rk1 = r;
                r = MinusMatrix(rk1, MultiplyMatrix(MultiplyMatrixByNumber(A, alpha), p));
                
                beta = (MultiplyMatrix(trans_matrix(r), r)[0, 0]) / (MultiplyMatrix(trans_matrix(rk1), rk1)[0, 0]);
                
                p = AddMatrix(r, MultiplyMatrixByNumber(p, beta));
                
                k += 1;
            }

            if (A.GetLength(0) == 2)
            {
                Console.WriteLine("Result in gradient method = : ");
                print(x);
            }
            sw.Stop();
            Console.WriteLine("Time for gradient method with threads = {0}", sw.ElapsedMilliseconds);
        }

        static void GradientMethodWithThreads(double[,] A, double[,] b)
        {
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            Thread[] threads = new Thread[A.GetLength(1)];
            double[,] x = new double[b.GetLength(0), b.GetLength(1)];
            
            double[,] r = MinusMatrix(b, MultiplyMatrix(A, x));
            
            double[,] p = r;
            int k = 0;
            double alpha = 0;
            double beta = 0; 
            
            
            while (k != A.GetLength(1))
            {     
                threads[k] = new Thread(() => MethodForGradient(ref A, ref b, ref x, ref r, ref p, ref alpha, ref beta));
                threads[k].Start();
                threads[k].Join();
                k += 1;
            }
            

            if (A.GetLength(0) == 2)
            {
                Console.WriteLine("Result in gradient method with threads = : ");
                print(x);
            }
            sw.Stop();
            Console.WriteLine("Time for gradient method  = {0}", sw.ElapsedMilliseconds);
        }

        static void MethodForGradient(ref double[,] A, ref double[,] b, ref double[,] x, ref double[,] r,
            ref double[,] p, ref double alpha, ref double beta)
        {
            alpha = (MultiplyMatrix(trans_matrix(r), r)[0, 0]) /
                    (MultiplyMatrix(MultiplyMatrix(trans_matrix(p), A), p)[0, 0]);
            x = AddMatrix(x, MultiplyMatrixByNumber(p, alpha));
            var rk1 = r;
            r = MinusMatrix(rk1, MultiplyMatrix(MultiplyMatrixByNumber(A, alpha), p));
            beta = (MultiplyMatrix(trans_matrix(r), r)[0, 0]) / (MultiplyMatrix(trans_matrix(rk1), rk1)[0, 0]);
            p = AddMatrix(r, MultiplyMatrixByNumber(p, beta));
        }
        static void print(double [,] x)
        {
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    Console.Write(x[i,j] + " ");
                }
                Console.WriteLine();
            }
        }
        static double [,] trans_matrix(double[,] matrix)
        {
            double [,] trans_matrix = new double[matrix.GetLength(1), matrix.GetLength(0)];
            
            for(int i = 0; i<matrix.GetLength(0); i++){
                for(int j = 0; j<matrix.GetLength(1); j++){
                    trans_matrix[j,i] = matrix[i,j];
                }
            }
            return trans_matrix;
        }
        
        static double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            if (cA != rB)
            {
                Console.WriteLine("Matrixes can't be multiplied!!");
                return A;
            }
            else
            {
                double temp = 0;
                double[,] kHasil = new double[rA, cB];

                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
                
            }
        }

        static double[,] MultiplyMatrixByNumber(double[,] A, double number)
        {
            double [,] new_a = new double[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < new_a.GetLength(0); i++)
            {
                for (int j = 0; j < new_a.GetLength(1); j++)
                {
                    new_a[i, j] = A[i, j] * number;
                }
            }
            return new_a;
        }
        
        static double[,] AddMatrix(double[,] A, double[,] B)
        {
            double [,] new_a = new double[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < new_a.GetLength(0); i++)
            {
                for (int j = 0; j < new_a.GetLength(1); j++)
                {
                    new_a[i, j] = A[i, j] + B[i,j];
                }
            }
            return new_a;
        }

        static double[,] MinusMatrix(double[,] A, double[,] B)
        {
            double [,] new_a = new double[A.GetLength(0), A.GetLength(1)];
            for (int i = 0; i < new_a.GetLength(0); i++)
            {
                for (int j = 0; j < new_a.GetLength(1); j++)
                {
                    new_a[i, j] = A[i,j] - B[i,j];
                }
            }
            return new_a;
        }

        static void Main(string[] args)
        {
            
            double[,] A_1 =
            {
                {4, 1},
                {1, 3}
            };
            double[,] b_1 =
            {
                {1},
                {2}
            };
            
            int diimension = 1000;
            double[] B_1 = {1.0, 2};
            Console.WriteLine("\n\n\nResult check:\n\n");
            GradientMethod(A_1, b_1);
            GradientMethodWithThreads(A_1, b_1);
            GaussMethod(A_1, B_1);
            GaussWithThreads(A_1, B_1, 2);
            
            int dimension = 100;
            
            double[,] A = new double[dimension, dimension];
            double[,] b = new double[dimension, dimension];
            for (int i = 0; i < dimension/2; i++) 
            { 
                for (int j = 0; j < dimension/2; j++) 
                { 
                    if(i == j)
                    {
                        A[i, j] = Math.Round(i * 2 + j * 3.0, 2);
                    }
                    else
                    {
                        A[i, j] = Math.Round(i * 7 + j * 2.0, 2);
                        A[j, i] = A[i, j];
                    }
                }
                b[i,0] = Math.Round(i * 2.0, 2); 
            } 
            
            double[] B = new double[diimension];
            double[,] A_g = new double[diimension, diimension];
            
            
            Random random = new();
            for (int i = 0; i < diimension; i++)
            {
                B[i] = random.Next(1, 10);
                for (int j = 0; j < diimension; j++)
                {
                    A_g[i, j] = random.Next(1, 10);
                }
            }
            Console.WriteLine("\n\n\nTime check:\n\n");
            
            GradientMethodWithThreads(A, b);
            GradientMethod(A, b);
            GaussMethod(A_g, B);
            GaussWithThreads(A_g, B, 2);
        }
    }
}
