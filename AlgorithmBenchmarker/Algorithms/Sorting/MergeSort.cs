using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class MergeSort : IAlgorithm
    {
        public string Name => "Merge Sort";
        public string Category => "Sorting";
        public string Complexity => "O(N log N)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is int[] intArray)
            {
                Sort(intArray, 0, intArray.Length - 1);
            }
            else if (input is float[] floatArray)
            {
                Sort(floatArray, 0, floatArray.Length - 1);
            }
            else if (input is string[] stringArray)
            {
                Sort(stringArray, 0, stringArray.Length - 1);
            }
            else
            {
                throw new ArgumentException("Unsupported input type for Merge Sort");
            }
        }

        private void Sort<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                Sort(array, left, mid);
                Sort(array, mid + 1, right);
                Merge(array, left, mid, right);
            }
        }

        private void Merge<T>(T[] array, int left, int mid, int right) where T : IComparable<T>
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            T[] L = new T[n1];
            T[] R = new T[n2];

            Array.Copy(array, left, L, 0, n1);
            Array.Copy(array, mid + 1, R, 0, n2);

            int i = 0, j = 0;
            int k = left;

            while (i < n1 && j < n2)
            {
                if (L[i].CompareTo(R[j]) <= 0)
                {
                    array[k] = L[i];
                    i++;
                }
                else
                {
                    array[k] = R[j];
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                array[k] = L[i];
                i++;
                k++;
            }

            while (j < n2)
            {
                array[k] = R[j];
                j++;
                k++;
            }
        }
    }
}
