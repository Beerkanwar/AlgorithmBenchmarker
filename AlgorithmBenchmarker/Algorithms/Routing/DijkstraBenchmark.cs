using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    // Need a weighted graph. We'll reuse GraphData but assume weights=1 or modify GraphData?
    // Let's subclass or just create local simple weighted graph struct for Dijkstra
    // But InputGenerator produces GraphData. Let's adapt GraphData to support weights or just random weights on fly.
    
    public class DijkstraBenchmark : IAlgorithm
    {
        public string Name => "Dijkstra Shortest Path";
        public string Category => "Routing";
        public string Complexity => "O(E + V log V)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is GraphData graph)
            {
                 // Run Dijkstra from node 0
                 // Since GraphData has no weights, we assume weight = 1 (BFS equivalent) 
                 // OR we deterministically derive weight from (u+v)%10 to simulate weights.
                 RunDijkstra(graph, 0);
            }
        }

        private void RunDijkstra(GraphData graph, int startNode)
        {
            int n = graph.Vertices;
            if (n == 0) return;

            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
            dist[startNode] = 0;

            // Priority Queue is standard for Dijkstra. 
            // .NET 6+ has PriorityQueue<TElement, TPriority>
            var pq = new PriorityQueue<int, int>();
            pq.Enqueue(startNode, 0);

            while (pq.Count > 0)
            {
                int u = pq.Dequeue();

                foreach (var v in graph.AdjacencyList[u])
                {
                    // Simulated Weight
                    int weight = (u + v) % 10 + 1; 

                    if (dist[u] != int.MaxValue && dist[u] + weight < dist[v])
                    {
                        dist[v] = dist[u] + weight;
                        pq.Enqueue(v, dist[v]);
                    }
                }
            }
        }
    }
}
