using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class HashTableLookup : IAlgorithm
    {
        public string Name => "Hash Table Lookup";
        public string Category => "Indexing";
        public string Complexity => "O(1) Avg";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is IndexingInputData data)
            {
                // Build Phase (O(N)) - usually excluded or included?
                // Benchmark usually measures Query Time.
                // But InputGenerator provides Arrays.
                // If we build inside Execute, we measure Build + Query.
                // For Indexing, Build time is relevant.
                var table = new HashSet<int>(data.Dataset);
                
                foreach (var query in data.SearchQueries)
                {
                    bool found = table.Contains(query);
                }
            }
        }
    }
}
