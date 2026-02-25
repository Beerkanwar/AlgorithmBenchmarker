using System;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Algorithms.Graph;

namespace AlgorithmBenchmarker.Algorithms.Routing
{
    public class DistanceVector : IAlgorithm
    {
        public string Name => "Distance Vector (Sim)";
        public string Category => "Routing";
        public string Complexity => "O(V*E)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is EnhancedGraphData graph)
            {
                // Simulate Distance Vector Protocol (Bellman Ford distributed style)
                // We just run one iteration of updates across all nodes to simulate a "round"
                // Or run convergence.
                // Let's run full Bellman Ford from Node 0 as simulation of its DV table.
                
                int V = graph.Vertices;
                int[] dist = new int[V];
                for(int i=0; i<V; i++) dist[i] = 1000000;
                dist[0] = 0;

                // Relax edges V-1 times
                for(int i=1; i<V; i++)
                {
                    bool changed = false;
                    for(int u=0; u<V; u++)
                    {
                        foreach(var edge in graph.WeightedAdjacencyList[u])
                        {
                            int v = edge.Item1;
                            int w = edge.Item2;
                            if (dist[u] != 1000000 && dist[u] + w < dist[v])
                            {
                                dist[v] = dist[u] + w;
                                changed = true;
                            }
                        }
                    }
                    if(!changed) break; 
                }
            }
        }
    }
}
