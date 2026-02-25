using System;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.Algorithms.Graph
{
    // Simple Graph representation for the benchmark
    public class GraphData
    {
        public int Vertices { get; set; }
        public List<List<int>> AdjacencyList { get; set; } = new List<List<int>>();

        public GraphData(int vertices)
        {
            Vertices = vertices;
            for (int i = 0; i < vertices; i++)
            {
                AdjacencyList.Add(new List<int>());
            }
        }

        public void AddEdge(int u, int v)
        {
            AdjacencyList[u].Add(v);
        }
    }

    public class BFS : IAlgorithm
    {
        public string Name => "BFS Traversal";
        public string Category => "Graph";
        public string Complexity => "O(V + E)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is GraphData graph)
            {
                Traverse(graph, 0);
            }
            else
            {
                throw new ArgumentException("Input must be GraphData for BFS");
            }
        }

        private void Traverse(GraphData graph, int startNode)
        {
            if (graph.Vertices == 0) return;

            bool[] visited = new bool[graph.Vertices];
            Queue<int> queue = new Queue<int>();

            visited[startNode] = true;
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();

                foreach (int v in graph.AdjacencyList[u])
                {
                    if (!visited[v])
                    {
                        visited[v] = true;
                        queue.Enqueue(v);
                    }
                }
            }
        }
    }
}
