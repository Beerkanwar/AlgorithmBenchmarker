using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    public class AStar : IAlgorithm
    {
        public string Name => "A* Pathfinding";
        public string Category => "Routing";
        public string Complexity => "O(E)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                RunAStar(graph, 0, graph.Vertices - 1);
            }
        }

        private void RunAStar(EnhancedGraphData graph, int start, int goal)
        {
            int V = graph.Vertices;
            var openSet = new SortedSet<(int fScore, int u)>();
            var gScore = new Dictionary<int, int>();
            
            for(int i=0; i<V; i++) gScore[i] = int.MaxValue;
            gScore[start] = 0;
            
            openSet.Add((Heuristic(start, goal), start));

            while (openSet.Count > 0)
            {
                var current = openSet.Min;
                openSet.Remove(current);
                int u = current.u;

                if (u == goal) return;

                foreach (var edge in graph.WeightedAdjacencyList[u])
                {
                    int v = edge.Item1;
                    int weight = edge.Item2;
                    int tentativeG = gScore[u] + weight;

                    if (tentativeG < gScore[v])
                    {
                        openSet.Remove((gScore[v] + Heuristic(v, goal), v)); // Approximate removal
                        gScore[v] = tentativeG;
                        openSet.Add((gScore[v] + Heuristic(v, goal), v));
                    }
                }
            }
        }

        private int Heuristic(int u, int target)
        {
            // Simple difference heuristic for benchmark
            return Math.Abs(target - u);
        }
    }
}
