namespace AlgorithmBenchmarker.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; }
        string Category { get; }
        string Complexity { get; }
        void Execute(object input);
    }
}
