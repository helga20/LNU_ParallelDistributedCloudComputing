using DotNetty.Common.Utilities;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Parallel6_Dijkstra
{
    internal class Program
    {
        static int INF = 99999;
        private static Dictionary<int, Dictionary<int, int>> GenerateGraph(int size)
        {
            Random random = new Random();
            var graph = new Dictionary<int, Dictionary<int, int>>();

            for (int i = 0; i < size; i++)
            {
                graph[i] = new Dictionary<int, int>();
                int numberOfEdges = random.Next(1, 11); // Each vertex has 1 to 10 edges

                for (int j = 0; j < numberOfEdges; j++)
                {
                    int randomVertex = random.Next(size);
                    if (randomVertex != i && !graph[i].ContainsKey(randomVertex))
                    {
                        int weight = random.Next(1, 101); // Edge weight between 1 and 100
                        graph[i][randomVertex] = weight;
                    }
                }
            }

            return graph;
        }
        private static TimeSpan RunSequentialDijkstra(Dictionary<int, Dictionary<int, int>> graph, int sourceNode)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var nodeDistances = new Dictionary<int, int>();
            var nodeVisited = new Dictionary<int, bool>();
            int vertexCount = graph.Count;
            for (int i = 0; i < vertexCount; i++)
            {
                nodeDistances[i] = int.MaxValue;
                nodeVisited[i] = false;
            }
            nodeDistances[sourceNode] = 0;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                int minimalDistance = int.MaxValue;
                int currentNode = -1;

                foreach (var node in nodeDistances)
                {
                    if (!nodeVisited[node.Key] && node.Value <= minimalDistance)
                    {
                        minimalDistance = node.Value;
                        currentNode = node.Key;
                    }
                }
                if (currentNode != -1)
                {
                    nodeVisited[currentNode] = true;
                    foreach (var edge in graph[currentNode])
                    {
                        int updatedDistance = nodeDistances[currentNode] + edge.Value;
                        if (updatedDistance < nodeDistances[edge.Key])
                        {
                            nodeDistances[edge.Key] = updatedDistance;
                        }
                    }
                }
            }

            stopWatch.Stop();
            
            Console.WriteLine($"Sequential time ~ {stopWatch.Elapsed}");
            return stopWatch.Elapsed;
        }

        public static TimeSpan RunParallelDijkstra(Dictionary<int, Dictionary<int, int>> graph, int startNode, int maxParallelism)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var nodeDistances = new ConcurrentDictionary<int, int>();
            var nodeVisited = new ConcurrentDictionary<int, bool>();

            for (int i = 0; i < graph.Count; i++)
            {
                nodeDistances[i] = int.MaxValue;
                nodeVisited[i] = false;
            }
            nodeDistances[startNode] = 0;

            int completedNodesCount = 0;

            while (completedNodesCount < graph.Count - 1)
            {
                int minimalDistance = int.MaxValue;
                int currentNode = -1;

                Parallel.ForEach(Partitioner.Create(0, graph.Count, graph.Count / maxParallelism), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        if (!nodeVisited[i] && nodeDistances[i] <= minimalDistance)
                        {
                            lock (nodeVisited)
                            {
                                if (!nodeVisited[i] && nodeDistances[i] <= minimalDistance)
                                {
                                    minimalDistance = nodeDistances[i];
                                    currentNode = i;
                                }
                            }
                        }
                    }
                });

                if (currentNode != -1)
                {
                    nodeVisited[currentNode] = true;
                    completedNodesCount++;

                    var currentEdges = graph[currentNode];

                    Parallel.ForEach(Partitioner.Create(0, currentEdges.Count), range =>
                    {
                        int localIndex = 0;
                        foreach (var edge in currentEdges)
                        {
                            if (localIndex >= range.Item1 && localIndex < range.Item2)
                            {
                                int updatedDistance = nodeDistances[currentNode] + edge.Value;
                                if (updatedDistance < nodeDistances[edge.Key])
                                {
                                    lock (nodeDistances)
                                    {
                                        if (updatedDistance < nodeDistances[edge.Key])
                                        {
                                            nodeDistances[edge.Key] = updatedDistance;
                                        }
                                    }
                                }
                            }
                            localIndex++;
                        }
                    });
                }
            }

            stopWatch.Stop();
            Console.WriteLine($"Parallel time ~ {stopWatch.Elapsed} with {maxParallelism} threads");
            return stopWatch.Elapsed;
        }
        static void Main(string[] args)
        {
            int n = 35000; //max 35000
            int threads = 4;
            var Graph = GenerateGraph(n);

            var graph = new Dictionary<int, Dictionary<int, int>>()
{
    { 0, new Dictionary<int, int>() { { 1, 5 }, { 3, 10 } } },
    { 1, new Dictionary<int, int>() { { 2, 3 } } },
    { 2, new Dictionary<int, int>() { { 3, 1 } } },
    { 3, new Dictionary<int, int>() }
};
            int source = 0;

            Console.WriteLine($"Vertexes = {n}");

            TimeSpan sync = RunSequentialDijkstra(Graph, source);
            TimeSpan async = RunParallelDijkstra(Graph, source, threads);

            var acceleration = sync / async;
            var efficiency = acceleration / threads;
            Console.WriteLine($"Acceleration ~ {acceleration}");
            Console.WriteLine($"Efficiency ~ {efficiency}");

        }
    }
}