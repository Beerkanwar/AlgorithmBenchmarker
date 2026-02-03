namespace AlgorithmBenchmarker.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; }
        string Category { get; }
        void Execute(object input);
    }
}
