using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Lab2
{
    internal class Program
    {
        public delegate double[,] ExampleDelegate(double[,] matrix);

        static double[,] TaskFunction(double[,] matrix)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] % 2 != 0)
                    {
                        matrix[i, j] = Math.Round(Math.Tan(matrix[i, j]) + (1 / Math.Tan(matrix[i, j])), 3);
                    }
                }
            }
            sw.Stop();
            Console.WriteLine("Task Function Time = {0}", sw.ElapsedMilliseconds);
            //PrintMatrix(matrix);
            return matrix;
        }

        static double[,] TaskFunctionAsync(double[,] matrix)
        {
            Stopwatch sw = new Stopwatch();
            var tasks = new List<Task>();
            sw.Start();
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                Task task = new Task(o =>
                {
                    int c = (int)o;
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[c, j] % 2 != 0)
                        {
                            matrix[c, j] = Math.Round(Math.Tan(matrix[c, j]) + (1 / Math.Tan(matrix[c, j])), 3);
                        }
                    }
                }, i);
                tasks.Add(task);
                task.Start();
            }
            Task.WaitAll(tasks.ToArray());
            sw.Stop();

            Console.WriteLine("Task Function Async Time = {0}", sw.ElapsedMilliseconds);
            //PrintMatrix(matrix);
            return matrix;
        }

        static void OnComplete()
        {
            Console.WriteLine("This is a callback! The Task was executed in thread {0}", Thread.CurrentThread.ManagedThreadId);
        }
        static void PrintMatrix(double[,] matrix)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {

            int dimension = 10000;
            double[,] matrix = new double[dimension, dimension];
            double[,] matrix2 = new double[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    matrix[i, j] = i * 2 + j;
                    matrix2[i, j] = i * 2 + j;
                }

            }

            Console.WriteLine("matrix");
            //PrintMatrix(matrix);

            ExampleDelegate del = TaskFunctionAsync;
            ExampleDelegate del2 = TaskFunction;
            del2.Invoke(matrix2);


            Task task = new Task(o =>
            {
                double[,] m = (double[,])o;
                del.Invoke(m);
            }, matrix);
            task.ContinueWith((act) => { OnComplete(); });
            task.Start();
            Task.WaitAll(task);
            Console.WriteLine("The End!");
            
        }       
    
    }
}
