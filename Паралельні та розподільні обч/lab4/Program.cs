using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;


namespace lab4._2
{
    static class Program
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
            
            /*Console.WriteLine("Result = : ");
            foreach (var x in X)
            {
                Console.Write(x + "  ");
            }*/
            sw.Stop();
            Console.WriteLine("Time for Gausse method = {0}", sw.ElapsedMilliseconds);
        }

        public static void GaussWithThreads(double[,] A, double[] B,  int threadCount)
        {
            double[] X = new double[A.GetLength(0)];
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            int Size = A.GetLength(0);
            List<WaitCallback> callbacks = new List<WaitCallback>();
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
                    callbacks.Add(new WaitCallback(SubtractionFromMatrix));
                }
                for (int index = 0; index < threadCount; index++)
                {
                    ThreadPool.QueueUserWorkItem(callbacks[index], new object[] { A, Size, B, index, j, max  });
                }
            }
            List<WaitCallback> callbackss = new List<WaitCallback>();
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
                    callbackss.Add(new WaitCallback(GetResult));
                }
                for (int index = 0; index < threadCount; index++)
                {
                    ThreadPool.QueueUserWorkItem(callbackss[index], new object[] {A, Size, B, X, index, j   }); 
                }
               

            }
            /*Console.WriteLine("Result = : ");
            foreach (var x in X)
            {
                Console.Write(x + "  ");
            }*/
            sw.Stop();
            Console.WriteLine("Time for Gausse method with threads = {0}", sw.ElapsedMilliseconds);
        }
        public static void GetResult(object state)
        {
            object[] array = state as object[];
            double[,] A = (double[,])array[0];
            int Size = (int) array[1];
            double[] B = (double[])array[2];
            double[] X = (double[])array[3];
            int index = (int) array[4];
            int j = (int) array[5];
            
            for (int i = index; i < j; i += 4)
            {
                B[i] -= A[i, j] * X[j];
            }
        }
        public static void SubtractionFromMatrix(object state)
        {
            object[] array = state as object[];
            double[,] A = (double[,])array[0];
            int Size = (int) array[1];
            double[] B = (double[])array[2];
            int index = (int) array[3];
            int j = (int) array[4];
            double max = (double) array[5];
            for (int i = j + 1 + index; i < Size; i += 5)
            {
                double mult = A[i, j] / max;
                for (int k = j; k < Size; k++)
                {
                    A[i, k] -= mult * A[j, k];
                }
                B[i] -= mult * B[j];
            }
        }
        static void Main(string[] args)
        {
            int dimension = 1000;
            double[] B = new double[dimension];
            double[,] A = new double[dimension, dimension];
            
            
            Random random = new();
            for (int i = 0; i < dimension; i++)
            {
                B[i] = random.Next(1, 10);
                for (int j = 0; j < dimension; j++)
                {
                    A[i, j] = random.Next(1, 10);
                }
            }
            
            GaussMethod(A, B);
            GaussWithThreads(A, B, 10);
        }
    }
}