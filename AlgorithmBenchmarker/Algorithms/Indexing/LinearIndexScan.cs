using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class LinearIndexScan : IAlgorithm
    {
        public string Name => "Linear Index Scan";
        public string Category => "Indexing";
        public string Complexity => "O(N)";

        public void Execute(object input)
        {
            if (input is IndexingInputData data)
            {
                // Just scan array
                foreach (var query in data.SearchQueries)
                {
                    bool found = false;
                    for(int i=0; i<data.Dataset.Length; i++)
                    {
                        if (data.Dataset[i] == query)
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
