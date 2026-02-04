using System;
using System.Collections.Generic;
using System.Text;
using AlgorithmBenchmarker.Algorithms.Graph;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Services
{
    public class InputGenerator
    {
        private Random _random = new Random();

        public object GenerateInput(BenchmarkConfig config, string algorithmCategory)
        {
            // 1. Set Context for algorithms that need detailed settings (e.g. Encryption Mode)
            BenchmarkContext.Current = config;

            // 2. Routing / Graph
            if (algorithmCategory == "Graph" || algorithmCategory == "Routing")
            {
                return GenerateGraph(config);
            }

            // 3. Machine Learning
            if (algorithmCategory == "Machine Learning")
            {
                // Generate X/Y matrices
                return GenerateMLData(config);
            }
            if (algorithmCategory == "ML Optimization")
            {
                // MatrixMultBenchmark expects int (N)
                return config.InputSize;
            }

            // 4. Indexing
            if (algorithmCategory == "Indexing")
            {
                return GenerateIndexingData(config);
            }

            // 5. Dynamic Programming
            if (algorithmCategory == "Dynamic Programming")
            {
                // DP usually takes N (InputSize)
                return config.InputSize;
            }

            // 6. Compression
            if (algorithmCategory == "Compression")
            {
                if (config.CompressionInputType == "Binary")
                {
                    return GenerateByteArray(config.InputSize);
                }
                else // Text
                {
                    // Return string[] for GZipBenchmark compatibility
                    return GenerateStringArray(config.InputSize, DistributionType.Random);
                }
            }

            // 7. Encryption
            if (algorithmCategory == "Encryption")
            {
                // AESBenchmark expects int or string[]
                // New Encryption (RSA) will use int + Context
                return config.InputSize;
            }

            // 8. Sorting / Searching (Default)
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

        private int[] GenerateIntArray(int size, DistributionType distribution)
        {
            var array = new int[size];
            for (int i = 0; i < size; i++) array[i] = _random.Next(0, 100000); // Random 0-100k
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
            // For Compression/AES Text mode, we need longer strings maybe?
            // Current: GUID substring (8 chars). 
            // Let's create varying strings for realism?
            // "Sort" usually sorts keys. 8 chars is fine.
            // If Text Compression needs large text, we might need InputSize * Length.
            // But benchmark compares same input. 
            // Keep 8 chars for sorting/searching consistency.
            
            var array = new string[size];
            for (int i = 0; i < size; i++) 
            {
                array[i] = Guid.NewGuid().ToString().Substring(0, 8);
            }
            
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

        private EnhancedGraphData GenerateGraph(BenchmarkConfig config)
        {
            int vertices = config.InputSize;
            var graph = new EnhancedGraphData(vertices); // Uses Models.EnhancedGraphData
            
            // Map Density string to double
            double density = 0.1; // Sparse / Low
            if (config.GraphDensity == "Medium") density = 0.3;
            if (config.GraphDensity == "Dense" || config.GraphDensity == "High") density = 0.6;
            
            int maxEdges = vertices * (vertices - 1);
            if (!config.IsDirected) maxEdges /= 2;

            long targetEdges = (long)(maxEdges * density);
            
            // Cap edges to prevent freeze for large N (e.g. N=1000, e=1M is fine. N=10000 e=100M is slow)
            if (targetEdges > 2000000) targetEdges = 2000000; 
            if (targetEdges < vertices) targetEdges = vertices - 1; // Connect roughly

            // Random Graph Generation
            // For dense graphs, Hashset logic is needed to avoid duplicates efficiently.
            
            var existing = new HashSet<long>(); // Key: u * V + v (if V < 40000)
            
            for (int i = 0; i < targetEdges; i++)
            {
                int u = _random.Next(0, vertices);
                int v = _random.Next(0, vertices);
                
                if (u == v) continue;

                // Simple check
                if (!config.IsDirected && u > v) (u, v) = (v, u);

                // Add Edge
                int weight = config.IsWeighted ? _random.Next(1, 100) : 1;
                
                // Note: EnhancedGraphData.AddWeightedEdge handles Base AdjacencyList too.
                // It also handles Undirected logic internally if we pass directed=false.
                // BUT AddWeightedEdge logic for undirected calls base.AddEdge(v, u).
                // So we should call it once with directed=false.
                
                // We are not checking duplicates strictly to keep generator fast O(1), 
                // duplicates are acceptable for benchmarks (multigraph).
                
                graph.AddWeightedEdge(u, v, weight, config.IsDirected);
            }
            
            return graph;
        }

        private MLInputData GenerateMLData(BenchmarkConfig config)
        {
            int samples = config.InputSize;
            int features = config.FeatureDimension > 0 ? config.FeatureDimension : 10;
            
            double[][] X = new double[samples][];
            double[] y = new double[samples];
            
            for (int i = 0; i < samples; i++)
            {
                X[i] = new double[features];
                double sum = 0;
                for (int j = 0; j < features; j++)
                {
                    X[i][j] = _random.NextDouble();
                    sum += X[i][j] * (j+1); // Dummy linear relation
                }
                // Regression label + noise
                y[i] = sum + (_random.NextDouble() - 0.5); 
            }
            
            return new MLInputData(X, y);
        }

        private IndexingInputData GenerateIndexingData(BenchmarkConfig config)
        {
            int size = config.InputSize;
            int[] dataset = new int[size];
            for (int i = 0; i < size; i++) dataset[i] = _random.Next(0, size * 10);
            
            // Queries
            int queryCount = config.QueryCount > 0 ? config.QueryCount : 100;
            int[] queries = new int[queryCount];
            for (int i = 0; i < queryCount; i++)
            {
                // Mix of hits and misses
                if (_random.NextDouble() > 0.5)
                    queries[i] = dataset[_random.Next(0, size)]; // Hit
                else
                    queries[i] = _random.Next(0, size * 10); // Likely Miss
            }
            
            return new IndexingInputData(dataset, queries);
        }
    }
}
