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
        public long MemoryBytes { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
