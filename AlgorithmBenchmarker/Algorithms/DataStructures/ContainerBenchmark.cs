using System;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Models.Containers;

namespace AlgorithmBenchmarker.Algorithms.DataStructures
{
    public class ContainerBenchmark : IAlgorithm
    {
        public string Name => "Dynamic Container Swap Benchmark";
        public string Category => "Data Structures";
        public string Complexity => "Depends on Container";

        public void Execute(object input)
        {
            if (input is int[] items)
            {
                var config = BenchmarkContext.Current;
                IContainer<int> container = CreateContainer(config?.InjectedContainerType ?? "Array");

                // Identical algorithm logic across all containers:
                // 1. Insert all items
                foreach (var item in items)
                {
                    container.Insert(item);
                }

                // 2. Extract all items (sort/drain)
                while (!container.IsEmpty)
                {
                    container.Extract();
                }
            }
        }

        private IContainer<int> CreateContainer(string type)
        {
            switch (type)
            {
                case "BinaryHeap": return new BinaryHeapContainer<int>();
                case "PairingHeap": return new PairingHeapContainer<int>();
                case "FibonacciHeap": return new FibonacciHeapContainer<int>();
                case "Array": 
                default:
                    return new ArrayContainer<int>();
            }
        }
    }
}
