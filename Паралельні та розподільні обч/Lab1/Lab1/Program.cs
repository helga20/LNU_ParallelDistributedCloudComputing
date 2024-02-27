using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Lab1
{
    class Program 
    { 
        public delegate void ExampleDelegate(double[,] matrix, double number); 
 
        static void TaskFunction(double[,] matrix, double number) 
        { 
            Stopwatch sw = new Stopwatch(); 
            sw.Start();
            int result = 0; 
            for (int i = 0; i < matrix.GetLength(1); i++) 
            { 
                for (int j = 0; j < matrix.GetLength(1); j++) 
                { 
                    if (matrix[i, j] == number) 
                    { 
                        result += 1; 
                    } 
                } 
            } 
            sw.Stop();
            Console.WriteLine("Time = {0} result = {1}", sw.ElapsedMilliseconds, result);
        } 
 
        static void TaskFunctionAsync(double[,] matrix, double number) 
        {
            Stopwatch sw = new Stopwatch(); 
            var tasks = new List<Task>();
            sw.Start();
            int result = 0;
            int[] mas = new int[matrix.GetLength(1)];  
            for (int i = 0; i < matrix.GetLength(1); i++) 
            { 
                Task task = new Task(o => 
                { 
                    int c = (int)o;
                    for (int j = 0; j < matrix.GetLength(1); j++) 
                    { 
                        if (matrix[c, j] == number) 
                        { 
                            mas[c] += 1; 
                        } 
                    } 
                }, i); 
                task.Start(); 
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray()); 
            for (int i = 0; i < mas.Length; i++) 
            { 
                result += mas[i]; 
            } 
            sw.Stop();
            Console.WriteLine("TimeAsync = {0} result = {1}", sw.ElapsedMilliseconds, result); 
            
        } 
         
 
 
        static void Main(string[] args)
        {
            int dimension = 15000;
            double[,] matrix = new double[dimension, dimension]; 
            for (int i = 0; i < dimension; i++) 
            { 
                for (int j = 0; j < dimension; j++) 
                { 
                    matrix[i, j] = Math.Round(i * 0.2 + j * 0.3, 2); 
                } 
 
            } 
 
            double number = 1.4;

            ExampleDelegate del = TaskFunctionAsync; 
            del += TaskFunction; 
            del.Invoke(matrix, number); 
 
        } 
 
        static void PrintMatrix(double[,] matrix) 
        { 
            for (int i = 0; i < 5; i++) 
            { 
                for (int j = 0; j < 5; j++) 
                { 
                    Console.Write(matrix[i, j] + "  "); 
                } 
                Console.WriteLine(); 
            } 
        } 
    } 
}