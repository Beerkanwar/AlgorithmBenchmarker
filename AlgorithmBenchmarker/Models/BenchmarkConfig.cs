namespace AlgorithmBenchmarker.Models
{
    public enum InputType
    {
        Integer,
        Float,
        String
    }

    public enum DistributionType
    {
        Random,
        Sorted,
        ReverseSorted,
        NearlySorted
    }

    public class BenchmarkConfig
    {
        // New Range Parameters
        public int MinInputSize { get; set; } = 100;
        public int MaxInputSize { get; set; } = 1000;
        public int StepSize { get; set; } = 100;

        // Kept for simple single-run compatibility if needed, but we'll prioritize ranges
        public int InputSize { get; set; } = 1000; 
        
        public InputType InputType { get; set; } = InputType.Integer;
        public DistributionType Distribution { get; set; } = DistributionType.Random;
        public int Repetitions { get; set; } = 5;

        // Encryption
        public int KeySize { get; set; } = 256;
        public string CipherMode { get; set; } = "CBC";
        public int BlockSize { get; set; } = 128;

        // Compression
        public string CompressionLevel { get; set; } = "Optimal"; // Fast, Optimal, NoCompression
        public string CompressionInputType { get; set; } = "Text"; // Text, Binary

        // Searching
        public bool EnforceSortedInput { get; set; } = true;
        public string TargetPosition { get; set; } = "Average"; // Best, Average, Worst

        // Dynamic Programming
        public bool UseMemoization { get; set; } = true;

        // Graph
        public string GraphDensity { get; set; } = "Sparse"; // Sparse, Dense
        public bool IsDirected { get; set; } = false;
        public bool IsWeighted { get; set; } = false;

        // Indexing
        public int QueryCount { get; set; } = 100;
        public string KeyDistribution { get; set; } = "Uniform";

        // Machine Learning
        public int FeatureDimension { get; set; } = 10;
        public int Epochs { get; set; } = 10;
        public int BatchSize { get; set; } = 32;

        // Routing
        public string CostMetric { get; set; } = "Distance";
    }
}
