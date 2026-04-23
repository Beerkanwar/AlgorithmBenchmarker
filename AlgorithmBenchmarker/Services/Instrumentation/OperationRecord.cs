namespace AlgorithmBenchmarker.Services.Instrumentation
{
    public enum OperationType
    {
        Comparison,
        Assignment,
        Swap,
        Traversal
    }

    /// <summary>
    /// Immutable operation record structure for deterministic instruction-level tracing.
    /// </summary>
    public readonly struct OperationRecord
    {
        public OperationType Type { get; }
        public string Details { get; }

        public OperationRecord(OperationType type, string details = "")
        {
            Type = type;
            Details = details;
        }

        public override string ToString() => $"{Type}: {Details}";
    }
}
