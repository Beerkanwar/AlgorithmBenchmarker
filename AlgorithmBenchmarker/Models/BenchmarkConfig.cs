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
    }
}
