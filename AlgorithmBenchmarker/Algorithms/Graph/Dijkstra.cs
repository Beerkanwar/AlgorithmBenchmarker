using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    public class Dijkstra : IAlgorithm
    {
        public string Name => "Dijkstra Shortest Path";
        public string Category => "Graph"; // Or Routing? Prompt says Add to Graph: Dijkstra.
        public string Complexity => "O(E log V)";

        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                RunDijkstra(graph, 0);
            }
        }

        private void RunDijkstra(EnhancedGraphData graph, int src)
        {
            int V = graph.Vertices;
            int[] dist = new int[V];
            for (int i = 0; i < V; i++) dist[i] = int.MaxValue;
            dist[src] = 0;

            // Priority Queue (SortedSet for simplicity, though not optimal for duplicates)
            // Using Custom list for benchmark simplicity of dependencies
            var pq = new SortedSet<(int distance, int u)>();
            pq.Add((0, src));

            while (pq.Count > 0)
            {
                var current = pq.Min;
                pq.Remove(current);
                int u = current.u;
                int d = current.distance;

                if (d > dist[u]) continue;

                foreach (var edge in graph.WeightedAdjacencyList[u])
                {
                    int v = edge.Item1;
                    int weight = edge.Item2;

                    if (dist[u] + weight < dist[v])
                    {
                        // Remove old if exists (UpdateKey workaround)
                        pq.Remove((dist[v], v)); 
                        dist[v] = dist[u] + weight;
                        pq.Add((dist[v], v));
                    }
                }
            }
        }
    }
}
