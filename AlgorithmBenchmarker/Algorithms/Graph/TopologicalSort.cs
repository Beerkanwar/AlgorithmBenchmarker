using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    public class TopologicalSort : IAlgorithm
    {
        public string Name => "Topological Sort";
        public string Category => "Graph";
        public string Complexity => "O(V + E)";

        public void Execute(object input)
        {
            if (input is GraphData graph)
            {
                // Ensure DAG? If cycles exist, typical algo produces incomplete or fails.
                // Assuming generator produces graph (might have cycles).
                // We'll run standard DFS based topo sort.
                
                Stack<int> stack = new Stack<int>();
                bool[] visited = new bool[graph.Vertices];
                
                for (int i = 0; i < graph.Vertices; i++)
                {
                    if (!visited[i]) TopoUtils(graph, i, visited, stack);
                }
            }
        }

        private void TopoUtils(GraphData graph, int u, bool[] visited, Stack<int> stack)
        {
            visited[u] = true;
            foreach (int v in graph.AdjacencyList[u])
            {
                if (!visited[v]) TopoUtils(graph, v, visited, stack);
            }
            stack.Push(u);
        }
    }
}
