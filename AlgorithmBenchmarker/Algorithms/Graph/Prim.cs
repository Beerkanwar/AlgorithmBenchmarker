using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    public class Prim : IAlgorithm
    {
        public string Name => "Prim's MST";
        public string Category => "Graph";
        public string Complexity => "O(E log V)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                RunPrim(graph);
            }
        }

        private void RunPrim(EnhancedGraphData graph)
        {
            int V = graph.Vertices;
            if (V == 0) return;

            int[] key = new int[V];
            bool[] mstSet = new bool[V];
            
            for (int i = 0; i < V; i++) key[i] = int.MaxValue;
            key[0] = 0;

            // Simplified Priority Queue
            var pq = new SortedSet<(int key, int u)>();
            pq.Add((0, 0));

            while (pq.Count > 0)
            {
                var min = pq.Min;
                pq.Remove(min);
                int u = min.u;

                mstSet[u] = true;

                foreach (var edge in graph.WeightedAdjacencyList[u])
                {
                    int v = edge.Item1;
                    int weight = edge.Item2;

                    if (!mstSet[v] && weight < key[v])
                    {
                        pq.Remove((key[v], v));
                        key[v] = weight;
                        pq.Add((key[v], v));
                    }
                }
            }
        }
    }
}
