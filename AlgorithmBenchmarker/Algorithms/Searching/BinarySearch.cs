using System;

namespace AlgorithmBenchmarker.Algorithms.Searching
{
    public class BinarySearch : IAlgorithm
    {
        public string Name => "Binary Search";
        public string Category => "Searching";
        public string Complexity => "O(log N)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            // Assuming input is the array, and we search for the item at index Length/2
            if (input is int[] intArray)
            {
                int target = intArray[intArray.Length / 2];
                Search(intArray, target);
            }
            else if (input is float[] floatArray)
            {
                float target = floatArray[floatArray.Length / 2];
                Search(floatArray, target);
            }
            else if (input is string[] stringArray)
            {
                string target = stringArray[stringArray.Length / 2];
                Search(stringArray, target);
            }
            else
            {
                throw new ArgumentException("Unsupported input type for Binary Search");
            }
        }

        private int Search<T>(T[] array, T target) where T : IComparable<T>
        {
            int left = 0;
            int right = array.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int comparison = array[mid].CompareTo(target);

                if (comparison == 0)
                {
                    return mid;
                }

                if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }
    }
}
