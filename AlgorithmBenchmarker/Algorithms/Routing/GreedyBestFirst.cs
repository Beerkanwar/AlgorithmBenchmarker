using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    public class GreedyBestFirst : IAlgorithm
    {
        public string Name => "Greedy Best-First";
        public string Category => "Routing";
        public string Complexity => "O(E)";

        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                RunGBFS(graph, 0, graph.Vertices - 1);
            }
        }

        private void RunGBFS(EnhancedGraphData graph, int start, int goal)
        {
            var openSet = new SortedSet<(int h, int u)>();
            var visited = new HashSet<int>();
            
            openSet.Add((Heuristic(start, goal), start));
            visited.Add(start);

            while (openSet.Count > 0)
            {
                var current = openSet.Min;
                openSet.Remove(current);
                int u = current.u;

                if (u == goal) return;

                foreach (var edge in graph.WeightedAdjacencyList[u])
                {
                    int v = edge.Item1;
                    if (!visited.Contains(v))
                    {
                        visited.Add(v);
                        openSet.Add((Heuristic(v, goal), v));
                    }
                }
            }
        }

        private int Heuristic(int u, int target) => Math.Abs(target - u);
    }
}
