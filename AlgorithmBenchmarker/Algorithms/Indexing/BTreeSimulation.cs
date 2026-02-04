using System;
using System.Collections.Generic;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class BTreeSimulation : IAlgorithm
    {
        public string Name => "B-Tree Simulation";
        public string Category => "Indexing";
        public string Complexity => "O(log N)";

        public void Execute(object input)
        {
            if (input is IndexingInputData data)
            {
                // Full B-Tree implementation is complex.
                // We simulate B-Tree behavior using a SortedDictionary (Red-Black Tree, similar properties)
                // or just SortedList for O(log N) lookups.
                // Real B-Trees optimize disk I/O, not RAM. In RAM it's similar to BST/AVL.
                // We'll use SortedDictionary.
                
                var btree = new SortedDictionary<int, int>();
                
                foreach (var val in data.Dataset) 
                {
                    if (!btree.ContainsKey(val)) btree.Add(val, val);
                }

                foreach (var query in data.SearchQueries)
                {
                    bool found = btree.ContainsKey(query);
                }
            }
        }
    }
}
