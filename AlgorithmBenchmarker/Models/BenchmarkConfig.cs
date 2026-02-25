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

        // --- Advanced Research-Grade Toggles ---
        
        // 1. Adversarial Input
        public bool UseAdversarialInput { get; set; } = false;
        public string AdversarialStrategy { get; set; } = "Strictly Increasing";

        // 2. Micro-Operation Tracer
        public bool EnableMicroTracer { get; set; } = false;

        // 3. Thread Scaling (Amdahl)
        public bool EnableThreadScalingAnalysis { get; set; } = false;
        public int MaxThreadsForScaling { get; set; } = Environment.ProcessorCount;

        // 4. Container Swapping
        public bool EnableContainerSwapping { get; set; } = false;
        public string InjectedContainerType { get; set; } = "Array"; // BinaryHeap, PairingHeap, FibonacciHeap
        
        // 5. JIT Warmup Profiler
        public bool EnableJitWarmupProfiler { get; set; } = false;
        public int JitIterations { get; set; } = 100;
        public bool JitForceGc { get; set; } = false;

        // 6. Drag Race Mode
        public bool EnableDragRaceMode { get; set; } = false;

        // --- Phase 2: Advanced Research-Grade Toggles ---

        // 7. Energy Estimator
        public bool EnableEnergyEstimator { get; set; } = false;

        // 8. Cache Locality Analyzer
        public bool EnableCacheLocalityAnalyzer { get; set; } = false;
        public int CacheLineSizeBytes { get; set; } = 64;

        // 9. Theoretical Bounds Verification
        public bool EnableTheoreticalBoundsVerification { get; set; } = false;

        // 10. Phase Transition Detector
        public bool EnablePhaseTransitionDetector { get; set; } = false;
        public string PhaseTransitionSweepParameter { get; set; } = "Density"; // or KeySize, Constraint, etc.
        public double PhaseTransitionSweepStart { get; set; } = 0.1;
        public double PhaseTransitionSweepEnd { get; set; } = 1.0;
        public int PhaseTransitionSteps { get; set; } = 10;

        // 11. GC Topology Profiler
        public bool EnableGcTopologyProfiler { get; set; } = false;

        public BenchmarkConfig Clone()
        {
            return (BenchmarkConfig)this.MemberwiseClone();
        }
    }
}
