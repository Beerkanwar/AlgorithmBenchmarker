using System;

namespace AlgorithmBenchmarker.Algorithms.Searching
{
    public class ExponentialSearch : IAlgorithm
    {
        public string Name => "Exponential Search";
        public string Category => "Searching";
        public string Complexity => "O(log N)";

        public void Execute(object input)
        {
             if (input is int[] arr && arr.Length > 0)
            {
                int n = arr.Length;
                int x = arr[n - 1];
                
                if (arr[0] == x) return;
                int i = 1;
                while (i < n && arr[i] <= x) i = i * 2;
                
                int left = i / 2;
                int right = Math.Min(i, n - 1);
                
                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    if (arr[mid] == x) return;
                    if (arr[mid] < x) left = mid + 1;
                    else right = mid - 1;
                }
            }
        }
    }
}
