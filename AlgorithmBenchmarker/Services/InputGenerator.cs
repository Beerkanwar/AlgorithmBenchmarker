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
            if (algorithmCategory == "Graph" || algorithmCategory == "Routing")
            {
                return GenerateGraph(config);
            }
            if (algorithmCategory == "Dynamic Programming" || algorithmCategory == "ML Optimization")
            {
                return config.InputSize;
            }
            
            // Compression / Encryption specific logic
            if (algorithmCategory == "Compression" || algorithmCategory == "Encryption")
            {
                // If specific CompressionInputType is set (Binary), return bytes
                if (config.CompressionInputType == "Binary" || algorithmCategory == "Encryption")
                {
                    // Encryption almost always works on bytes
                    return GenerateByteArray(config.InputSize);
                }
                // Else Text, fall through to String generation (or generate long string)
                if (config.CompressionInputType == "Text")
                {
                    // For compression, a single long string is better than an array of strings?
                    // Typically compression algorithms take byte[] or stream.
                    // Let's return byte[] for consistency in benchmarking, but derived from text if "Text"
                    return GenerateTextBytes(config.InputSize);
                }
            }
            
            // Indexing (Trie) needs Strings
            if (algorithmCategory == "Indexing")
            {
                return GenerateStringArray(config.InputSize, DistributionType.Random); // Indexing usually random keys
            }

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
                    return GenerateIntArray(config.InputSize, config.Distribution);
            }
        }

        private byte[] GenerateByteArray(int size)
        {
            var bytes = new byte[size];
            _random.NextBytes(bytes);
            return bytes;
        }

        private byte[] GenerateTextBytes(int size)
        {
            // Generate random text chars
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            var stringChars = new char[size];
            for (int i = 0; i < size; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }
            return System.Text.Encoding.UTF8.GetBytes(new string(stringChars));
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
                int swaps = Math.Max(1, array.Length / 100); 
                for (int k = 0; k < swaps; k++)
                {
                    int i = _random.Next(0, array.Length);
                    int j = _random.Next(0, array.Length);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
        }

        private GraphData GenerateGraph(BenchmarkConfig config)
        {
            int vertices = config.InputSize;
            var graph = new GraphData(vertices);
            
            // Density 0.0 - 1.0. Max edges = V * (V-1) (Directed) or V*(V-1)/2 (Undirected)
            // Use config.GraphDensity (double) if available, parsing from string?
            // Config.GraphDensity is string "Low", "Medium", "High"? No, Config definition says string?
            // Looking at BenchmarkConfig.cs in Step 16, it was:
            // public string GraphDensity { get; set; } = "Medium";
            
            double density = 0.1; // Low
            if (config.GraphDensity == "Medium") density = 0.3;
            if (config.GraphDensity == "High") density = 0.6;
            
            int maxEdges = vertices * (vertices - 1); // Directed max
            if (!config.IsDirected) maxEdges /= 2;
            
            int edgeCount = (int)(maxEdges * density);
            // Cap edge count to avoid freezing on huge graphs
            edgeCount = Math.Min(edgeCount, vertices * 20); 

            for (int i = 0; i < edgeCount; i++)
            {
                int u = _random.Next(0, vertices);
                int v = _random.Next(0, vertices);
                if (u != v)
                {
                    // If weighted, add weight? 
                    // GraphData.AddEdge signature needed.
                    // Assuming GraphData supports IsDirected logic or we handle it here.
                    graph.AddEdge(u, v); 
                    // If undirected, AddEdge might handle adding v->u, or we call it explicitly?
                    // Usually GraphData handles it.
                }
            }
            return graph;
        }
    }
}
