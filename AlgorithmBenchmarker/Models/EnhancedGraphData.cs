using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Models
{
    public class EnhancedGraphData : GraphData
    {
        // Adjacency List with Weights: source -> (target, weight)
        public List<List<Tuple<int, int>>> WeightedAdjacencyList { get; set; } = new List<List<Tuple<int, int>>>();

        public EnhancedGraphData(int vertices) : base(vertices)
        {
            for (int i = 0; i < vertices; i++)
            {
                WeightedAdjacencyList.Add(new List<Tuple<int, int>>());
            }
        }

        public void AddWeightedEdge(int u, int v, int weight, bool directed)
        {
            // Update Base (Unweighted view) for BFS/DFS compatibility
            base.AddEdge(u, v);
            
            // Update Weighted
            WeightedAdjacencyList[u].Add(new Tuple<int, int>(v, weight));

            if (!directed)
            {
                base.AddEdge(v, u);
                WeightedAdjacencyList[v].Add(new Tuple<int, int>(u, weight));
            }
        }
    }
}
