using System;

namespace AlgorithmBenchmarker.Algorithms.Searching
{
    public class JumpSearch : IAlgorithm
    {
        public string Name => "Jump Search";
        public string Category => "Searching";
        public string Complexity => "O(√N)";

        public void Execute(object input)
        {
            if (input is int[] arr && arr.Length > 0)
            {
                int n = arr.Length;
                int target = arr[n - 1]; 
                int step = (int)Math.Sqrt(n);
                int prev = 0;

                while (arr[Math.Min(step, n) - 1] < target)
                {
                    prev = step;
                    step += (int)Math.Sqrt(n);
                    if (prev >= n) return;
                }

                while (arr[prev] < target)
                {
                    prev++;
                    if (prev == Math.Min(step, n)) return;
                }
                
                if (arr[prev] == target) return;
            }
        }
    }
}
