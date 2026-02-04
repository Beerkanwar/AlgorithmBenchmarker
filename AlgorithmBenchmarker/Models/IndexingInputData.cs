using System.Collections.Generic;

namespace AlgorithmBenchmarker.Models
{
    public class IndexingInputData
    {
        // Dataset to index
        public int[] Dataset { get; set; }
        
        // Keys to query
        public int[] SearchQueries { get; set; }

        public IndexingInputData(int[] dataset, int[] queries)
        {
            Dataset = dataset;
            SearchQueries = queries;
        }
    }
}
