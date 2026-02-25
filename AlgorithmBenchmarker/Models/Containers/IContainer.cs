using System;

namespace AlgorithmBenchmarker.Models.Containers
{
    /// <summary>
    /// Container abstraction injection interface.
    /// Allows runtime substitution of underlying data structures.
    /// </summary>
    public interface IContainer<T> where T : IComparable<T>
    {
        string Name { get; }
        void Insert(T item);
        T Extract();
        bool IsEmpty { get; }
        long GetMemoryFootprintBytes();
    }
}
