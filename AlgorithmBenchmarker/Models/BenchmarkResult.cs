using System;

namespace AlgorithmBenchmarker.Models
{
    public class BenchmarkResult
    {
        public int Id { get; set; } // For DB
        public string BatchId { get; set; } = string.Empty; // Group ID for a single chart line
        public string AlgorithmName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int InputSize { get; set; }
        public double AvgTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public double StdDevTimeMs { get; set; }
        public long AllocatedBytes { get; set; }
        public long MemoryBytes { get; set; } // Keeping for back-compat, maybe deprecated or used for peak working set
        public DateTime Timestamp { get; set; }

        // --- Advanced Research-Grade Outputs ---
        public System.Collections.Generic.Dictionary<string, string> ExtendedMetrics { get; set; } = new System.Collections.Generic.Dictionary<string, string>();

        public string ExtendedMetricsDisplay
        {
            get
            {
                if (ExtendedMetrics == null || ExtendedMetrics.Count == 0) return string.Empty;
                var pairs = new System.Collections.Generic.List<string>();
                foreach (var kvp in ExtendedMetrics)
                {
                    pairs.Add($"{kvp.Key}: {kvp.Value}");
                }
                return string.Join(" | ", pairs);
            }
        }
    }
}
