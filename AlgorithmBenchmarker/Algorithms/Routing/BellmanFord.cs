using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    public class BellmanFord : IAlgorithm
    {
        public string Name => "Bellman-Ford Routing";
        public string Category => "Routing";
        public string Complexity => "O(VE)";

        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                int V = graph.Vertices;
                int[] dist = new int[V];
                for (int i = 0; i < V; i++) dist[i] = 1000000; // Infinity
                dist[0] = 0;

                for (int i = 1; i < V; ++i)
                {
                    for (int u = 0; u < V; ++u)
                    {
                        foreach (var edge in graph.WeightedAdjacencyList[u])
                        {
                            int v = edge.Item1;
                            int weight = edge.Item2;
                            if (dist[u] != 1000000 && dist[u] + weight < dist[v])
                                dist[v] = dist[u] + weight;
                        }
                    }
                }
            }
        }
    }
}
