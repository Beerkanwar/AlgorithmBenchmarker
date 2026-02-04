using System;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    public class FloydWarshall : IAlgorithm
    {
        public string Name => "Floyd-Warshall Routing";
        public string Category => "Routing";
        public string Complexity => "O(V^3)";

        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                int V = graph.Vertices;
                // Limit V for O(V^3)
                if (V > 500) V = 500; // Cap to avoid freeze
                
                int[,] dist = new int[V, V];
                int INF = 1000000;

                for (int i = 0; i < V; i++)
                    for (int j = 0; j < V; j++)
                    {
                         if (i == j) dist[i, j] = 0;
                         else dist[i, j] = INF;
                    }

                for (int u = 0; u < V; u++)
                {
                    foreach (var edge in graph.WeightedAdjacencyList[u])
                    {
                        int v = edge.Item1;
                        int w = edge.Item2;
                        if (v < V) dist[u, v] = w;
                    }
                }

                for (int k = 0; k < V; k++)
                {
                    for (int i = 0; i < V; i++)
                    {
                        for (int j = 0; j < V; j++)
                        {
                            if (dist[i, k] + dist[k, j] < dist[i, j])
                            {
                                dist[i, j] = dist[i, k] + dist[k, j];
                            }
                        }
                    }
                }
            }
        }
    }
}
