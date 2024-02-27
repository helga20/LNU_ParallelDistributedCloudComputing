using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Prima
{
    internal class Program
    {
        static void Main()
        {
            int numberOfVertices = 4; // Change this number to generate a larger matrix
            int source = 0; // Node 'a'
            while (numberOfVertices <= 4)
            {
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("GraphSize " + numberOfVertices);
                // Create a graph with the specified number of vertices
                Graph graph = new Graph(numberOfVertices);
                // Get the adjacency matrix of the generated graph
                var adjacencyMatrix = graph.GetAdjacencyList();

                // Call the Prim function to find the minimal spanning tree
                Stopwatch stopWatch = new();
                stopWatch.Start();
                Dictionary<int, int> minimalSpanningTree = Prim(adjacencyMatrix, source);

                stopWatch.Stop();
                var linTime = stopWatch.Elapsed;
                Console.WriteLine($"Prim's algorithm 'a' ({source}), time:{linTime}");
                foreach (var entry in minimalSpanningTree)
                {
                    Console.WriteLine($"From {entry.Key} to {entry.Value}");
                }

                // Call the ParalelPrim function
                int t = 2;
                for (; t <= 8; t *= 2)
                {
                    Stopwatch stopWatchAsync = new();
                    stopWatchAsync.Start();
                    Dictionary<int, int> minimalSpanningTree2 = ParallelPrim(adjacencyMatrix, source, t);
                    stopWatchAsync.Stop();
                    var parTime = stopWatchAsync.Elapsed;
                    Console.WriteLine($"Parallel t={t} 'a' ({source}) time: {parTime} acceleration: {linTime / parTime} efficiency: {(linTime / parTime) / t}");
                    foreach (var entry in minimalSpanningTree2)
                    {
                        Console.WriteLine($"From {entry.Key} to {entry.Value}");
                    }
                }
                numberOfVertices = (numberOfVertices * 5);
            }
        }

        private static Dictionary<int, int> Prim(
            Dictionary<int, Dictionary<int, int>> adjacencyMatrix,
            int source)
        {
            Dictionary<int, int> parents = new Dictionary<int, int>();
            Dictionary<int, int> keys = new Dictionary<int, int>();
            HashSet<int> unvisitedNodes = new HashSet<int>();
            foreach (int vertex in adjacencyMatrix.Keys)
            {
                keys[vertex] = int.MaxValue;
                unvisitedNodes.Add(vertex);
            }
            keys[source] = 0;
            while (unvisitedNodes.Count > 0)
            {
                int u = -1;
                foreach (int current in unvisitedNodes)
                {
                    if (u == -1 || keys[current] < keys[u])
                    {
                        u = current;
                    }
                }
                unvisitedNodes.Remove(u);
                foreach (var neighbor in adjacencyMatrix[u])
                {
                    int v = neighbor.Key;
                    int weight = neighbor.Value;

                    if (unvisitedNodes.Contains(v) && weight < keys[v])
                    {
                        parents[v] = u;
                        keys[v] = weight;
                    }
                }
            }
            return parents;
        }
        private static Dictionary<int, int> ParallelPrim(
            Dictionary<int, Dictionary<int, int>> adjacencyMatrix,
            int source, int threads)
        {
            Dictionary<int, int> parents = new Dictionary<int, int>();
            ConcurrentDictionary<int, int> keys = new ConcurrentDictionary<int, int>();
            ConcurrentDictionary<int, byte> unvisitedNodes = new ConcurrentDictionary<int, byte>();
            foreach (int vertex in adjacencyMatrix.Keys)
            {
                keys[vertex] = int.MaxValue;
                unvisitedNodes[vertex] = 0;
            }
            keys[source] = 0;
            while (unvisitedNodes.Count > 0)
            {
                int u = -1;
                int minKey = int.MaxValue;
                object lockObj = new object();

                // Parallelizing the loop to find the next vertex
                Parallel.ForEach(
                    unvisitedNodes.Keys,
                    new ParallelOptions { MaxDegreeOfParallelism = threads },
                    () => (vertex: -1, key: int.MaxValue),
                    (vertex, loopState, localState) =>
                    {
                        int vertexKey = keys[vertex];
                        if (vertexKey < localState.key)
                        {
                            localState.vertex = vertex;
                            localState.key = vertexKey;
                        }
                        return localState;
                    },
                    localState =>
                    {
                        lock (lockObj)
                        {
                            if (localState.key < minKey)
                            {
                                u = localState.vertex;
                                minKey = localState.key;
                            }
                        }
                    });

                if (u == -1)
                {
                    break;
                }
                unvisitedNodes.TryRemove(u, out _);
                // Iterate over neighbors and update the keys
                Parallel.ForEach(
                    adjacencyMatrix[u],
                    new ParallelOptions { MaxDegreeOfParallelism = threads },
                    neighborPair =>
                    {
                        int v = neighborPair.Key;
                        int weight = neighborPair.Value;

                        if (unvisitedNodes.ContainsKey(v) && weight < keys[v])
                        {
                            lock (lockObj)
                            {
                                if (unvisitedNodes.ContainsKey(v) && weight < keys[v])
                                {
                                    parents[v] = u;
                                    keys[v] = weight;
                                }
                            }
                        }
                    });
            }
            return parents;
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