using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    public class Kruskal : IAlgorithm
    {
        public string Name => "Kruskal's MST";
        public string Category => "Graph";
        public string Complexity => "O(E log E)";

        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                RunKruskal(graph);
            }
        }

        private void RunKruskal(EnhancedGraphData graph)
        {
            var edges = new List<(int u, int v, int w)>();
            int V = graph.Vertices;

            // Collect All Edges
            for (int u = 0; u < V; u++)
            {
                foreach (var edge in graph.WeightedAdjacencyList[u])
                {
                    int v = edge.Item1;
                    int w = edge.Item2;
                    if (u < v) edges.Add((u, v, w)); // Avoid duplicates for undirected
                }
            }

            edges.Sort((a, b) => a.w.CompareTo(b.w));
            
            var parent = new int[V];
            for (int i = 0; i < V; i++) parent[i] = i;

            int Find(int i)
            {
                if (parent[i] != i) parent[i] = Find(parent[i]);
                return parent[i];
            }

            void Union(int i, int j)
            {
                int rootI = Find(i);
                int rootJ = Find(j);
                if (rootI != rootJ) parent[rootI] = rootJ;
            }

            int edgesCount = 0;
            foreach (var edge in edges)
            {
                if (Find(edge.u) != Find(edge.v))
                {
                    Union(edge.u, edge.v);
                    edgesCount++;
                }
            }
        }
    }
}
