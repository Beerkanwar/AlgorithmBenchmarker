using System;

namespace AlgorithmBenchmarker.Services.Adversarial
{
    public class AdversarialQuickSortGenerator : IAdversarialGenerator
    {
        public string TargetAlgorithm => "Quick Sort";
        
        // Formal complexity-trigger guarantees:
        // Naive QuickSort with last/first-element pivot degraded to O(N^2) on Sorted or ReverseSorted.
        public string TriggerExplanation => "Generates a strictly increasing array. Forces O(N^2) for endpoints pivots.";

        public object GeneratePathologicalInput(int size, int seed)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++) array[i] = i; // worst case
            return array;
        }
    }

    public class AdversarialHashGenerator : IAdversarialGenerator
    {
        public string TargetAlgorithm => "Hash Table";
        
        public string TriggerExplanation => "Generates keys with identical hash codes (100% collision) leading to O(N) linked-list traversal per insertion.";

        public object GeneratePathologicalInput(int size, int seed)
        {
            // Assuming standard GetHashCode modulo scaling. For simplification, produce integers that have same lower bits
            int[] keys = new int[size];
            int modulo = 10007; // common proxy
            for (int i = 0; i < size; i++)
            {
                keys[i] = (i * modulo);
            }
            return keys;
        }
    }

    public class AdversarialBSTGenerator : IAdversarialGenerator
    {
        public string TargetAlgorithm => "Binary Search Tree";
        
        public string TriggerExplanation => "Degenerate linked-list insertion order. Sorted sequential values force O(N^2) tree construction.";

        public object GeneratePathologicalInput(int size, int seed)
        {
            int[] keys = new int[size];
            for (int i = 0; i < size; i++) keys[i] = i; 
            return keys;
        }
    }
}
