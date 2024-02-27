using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

namespace Dijcstra
{
    class Program
    {
        static void Main()
        {
            int numberOfVertices = 100; // Change this number to generate a larger matrix
            int source = 0; // Node 'a'
            while (numberOfVertices <= 100000)
            {
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("GraphSize " + numberOfVertices);
                // Create a graph with the specified number of vertices
                Graph graph = new Graph(numberOfVertices);
                // Get the adjacency matrix of the generated graph
                var adjacencyMatrix = graph.GetAdjacencyList();

                // Call the Dijkstra function to find the shortest paths
                Stopwatch stopWatch = new();
                stopWatch.Start();
                var shortestPaths = Dijkstra(adjacencyMatrix, source);
                
                stopWatch.Stop();
                var linTime = stopWatch.Elapsed;
                Console.WriteLine($"Iterational 'a' ({source}), time:{linTime}");
                //foreach (var path in shortestPaths)
                //{
                //    Console.WriteLine($"To vertex {path.Key}: {path.Value}");
                //}
                ////
                // Call the Dijkstra function to find the shortest paths
                int t = 2;
                for (; t <= 8; t *= 2)
                {
                   // ThreadPool.SetMaxThreads(t, t);
                    Stopwatch stopWatchAsync = new();
                    stopWatchAsync.Start();
                    var shortestPaths2 = ParallelDijkstra(adjacencyMatrix, source,t);
                    stopWatchAsync.Stop();
                    var parTime = stopWatchAsync.Elapsed;
                    //foreach (var path in shortestPaths2)
                    //{
                    //    Console.WriteLine($"To vertex {path.Key}: {path.Value}");
                    //}
                    Console.WriteLine($"Parallel t={t} 'a' ({source}) time: {parTime} acceleration: {linTime / parTime} efficiency: {(linTime / parTime) / t}");

                }
                numberOfVertices = (numberOfVertices * 5);
            }

        }




        public static Dictionary<int, int> Dijkstra(Dictionary<int, Dictionary<int, int>> adjacencyList
                    , int startVertex)
        {
            var distances = new Dictionary<int, int>();
            var unvisited = new Dictionary<int, bool>();
            var numberOfVertices = adjacencyList.Count;
            for (int i = 0; i < numberOfVertices; i++)
            {
                distances[i] = int.MaxValue;
                unvisited[i] = true;
            }
            distances[startVertex] = 0;

            for (int i = 0; i < numberOfVertices - 1; i++)
            {
                int minDistance = int.MaxValue;
                int currentVertex = -1;

                foreach (var vertex in distances)
                {
                    if (unvisited[vertex.Key] && vertex.Value <= minDistance)
                    {
                        minDistance = vertex.Value;
                        currentVertex = vertex.Key;
                    }
                }
                if (currentVertex != -1)
                {
                    unvisited[currentVertex] = false;
                    foreach (var edge in adjacencyList[currentVertex])
                    {
                        int newDistance = distances[currentVertex] + edge.Value;
                        if (newDistance < distances[edge.Key])
                        {
                            distances[edge.Key] = newDistance;
                        }
                    }
                }
            }
            return distances;
        }
        public static ConcurrentDictionary<int, int> ParallelDijkstra(Dictionary<int, Dictionary<int, int>> adjacencyList
                    , int startVertex, int maxDegreeOfParallelism)
        {
            var distances = new ConcurrentDictionary<int, int>();
            var unvisited = new ConcurrentDictionary<int, bool>();

            for (int i = 0; i < adjacencyList.Count; i++)
            {
                distances[i] = int.MaxValue;
                unvisited[i] = true;
            }
            distances[startVertex] = 0;

            int processedVertices = 0;

            while (processedVertices < adjacencyList.Count - 1)
            {
                int minDistance = int.MaxValue;
                int currentVertex = -1;

                Parallel.ForEach(Partitioner.Create(0, adjacencyList.Count, adjacencyList.Count / maxDegreeOfParallelism), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        if (unvisited[i] && distances[i] <= minDistance)
                        {
                            lock (unvisited)
                            {
                                if (unvisited[i] && distances[i] <= minDistance)
                                {
                                    minDistance = distances[i];
                                    currentVertex = i;
                                }
                            }
                        }
                    }
                });

                if (currentVertex != -1)
                {
                    unvisited[currentVertex] = false;
                    processedVertices++;

                    var currentEdges = adjacencyList[currentVertex];

                    Parallel.ForEach(Partitioner.Create(0, currentEdges.Count), range =>
                    {
                        int index = 0;
                        foreach (var edge in currentEdges)
                        {
                            if (index >= range.Item1 && index < range.Item2)
                            {
                                int newDistance = distances[currentVertex] + edge.Value;
                                if (newDistance < distances[edge.Key])
                                {
                                    lock (distances)
                                    {
                                        if (newDistance < distances[edge.Key])
                                        {
                                            distances[edge.Key] = newDistance;
                                        }
                                    }
                                }
                            }
                            index++;
                        }
                    });
                }
            }

            return distances;
        }
       
    }

    public class Graph
    {
        private Dictionary<int, Dictionary<int, int>> adjacencyList;

        public Graph(int numberOfVertices)
        {
            Random random = new Random();
            adjacencyList = new Dictionary<int, Dictionary<int, int>>();

            for (int i = 0; i < numberOfVertices; i++)
            {
                adjacencyList[i] = new Dictionary<int, int>();
                int numberOfEdges = random.Next(1, 11); // Each vertex has 1 to 10 edges

                for (int j = 0; j < numberOfEdges; j++)
                {
                    int randomVertex = random.Next(numberOfVertices);
                    if (randomVertex != i && !adjacencyList[i].ContainsKey(randomVertex))
                    {
                        int weight = random.Next(1, 101); // Edge weight between 1 and 100
                        adjacencyList[i][randomVertex] = weight;
                    }
                }
            }
        }

        public Dictionary<int, Dictionary<int, int>> GetAdjacencyList()
        {
            return adjacencyList;
        }
        public void PrintGraph()
        {
            foreach (var vertex in adjacencyList)
            {
                Console.Write($"Vertex {vertex.Key}: ");
                foreach (var edge in vertex.Value)
                {
                    Console.Write($"({edge.Key}, {edge.Value}) ");
                }
                Console.WriteLine();
            }
        }
    }
}