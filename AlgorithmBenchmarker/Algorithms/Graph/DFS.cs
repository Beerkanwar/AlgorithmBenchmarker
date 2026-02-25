using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph; // For GraphData

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    public class DFS : IAlgorithm
    {
        public string Name => "DFS Traversal";
        public string Category => "Graph";
        public string Complexity => "O(V + E)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is GraphData graph)
            {
                if (graph.Vertices == 0) return;
                bool[] visited = new bool[graph.Vertices];
                Traverse(graph, 0, visited);
            }
        }

        private void Traverse(GraphData graph, int u, bool[] visited)
        {
            visited[u] = true;
            foreach (int v in graph.AdjacencyList[u])
            {
                if (!visited[v]) Traverse(graph, v, visited);
            }
        }
    }
}
