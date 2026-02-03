using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmBenchmarker.Algorithms.Graph;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Services
{
    public class InputGenerator
    {
        private Random _random = new Random();

        public object GenerateInput(BenchmarkConfig config, string algorithmCategory)
        {
            // Special handling for Graph and DP
            if (algorithmCategory == "Graph" || algorithmCategory == "Routing")
            {
                return GenerateGraph(config.InputSize);
            }
            if (algorithmCategory == "Dynamic Programming" || algorithmCategory == "ML Optimization")
            {
                // For DP/ML, we treat InputSize as N (dimension or count)
                // ML needs N to create matrices internally or we can create them here.
                // Keeping it simple: Pass N.
                return config.InputSize;
            }
            // For others like Encryption/Compression/Indexing, we default to Arrays of standard types
            // defined in config.InputType (e.g. String array for Trie/Compression).


            // Default: generate arrays for Sorting/Searching
            switch (config.InputType)
            {
                case InputType.Integer:
                    return GenerateIntArray(config.InputSize, config.Distribution);
                case InputType.Float:
                    return GenerateFloatArray(config.InputSize, config.Distribution);
                case InputType.String:
                    return GenerateStringArray(config.InputSize, config.Distribution);
                default:
                    throw new ArgumentException("Unknown Input Type");
            }
        }

        private int[] GenerateIntArray(int size, DistributionType distribution)
        {
            var array = new int[size];
            for (int i = 0; i < size; i++) array[i] = _random.Next(0, 100000); // Random values

            ApplyDistribution(array, distribution);
            return array;
        }

        private float[] GenerateFloatArray(int size, DistributionType distribution)
        {
            var array = new float[size];
            for (int i = 0; i < size; i++) array[i] = (float)_random.NextDouble() * 100000f;

            ApplyDistribution(array, distribution);
            return array;
        }

        private string[] GenerateStringArray(int size, DistributionType distribution)
        {
            var array = new string[size];
            for (int i = 0; i < size; i++) array[i] = Guid.NewGuid().ToString().Substring(0, 8);

            ApplyDistribution(array, distribution);
            return array;
        }

        private void ApplyDistribution<T>(T[] array, DistributionType distribution) where T : IComparable<T>
        {
            if (distribution == DistributionType.Sorted)
            {
                Array.Sort(array);
            }
            else if (distribution == DistributionType.ReverseSorted)
            {
                Array.Sort(array);
                Array.Reverse(array);
            }
            else if (distribution == DistributionType.NearlySorted)
            {
                Array.Sort(array);
                // Swap a few elements to make it "nearly" sorted
                int swaps = Math.Max(1, array.Length / 100); 
                for (int k = 0; k < swaps; k++)
                {
                    int i = _random.Next(0, array.Length);
                    int j = _random.Next(0, array.Length);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
            // Random is default, do nothing
        }

        private GraphData GenerateGraph(int vertices)
        {
            var graph = new GraphData(vertices);
            // Create a random connected graph (roughly)
            // Or just random edges.
            // Let's add vertices * 2 edges
            int edges = vertices * 2;
            for (int i = 0; i < edges; i++)
            {
                int u = _random.Next(0, vertices);
                int v = _random.Next(0, vertices);
                if (u != v)
                {
                    graph.AddEdge(u, v);
                    // Undirected? BFS usually on directed or undirected. Let's do directed for simplicity as per implementation.
                }
            }
            return graph;
        }
    }
}
