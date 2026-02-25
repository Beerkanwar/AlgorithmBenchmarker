using System;

namespace AlgorithmBenchmarker.Algorithms.DynamicProgramming
{
    public class FibonacciDP : IAlgorithm
    {
        public string Name => "Fibonacci (DP)";
        public string Category => "Dynamic Programming";
        public string Complexity => "O(N)";
        public override string ToString() => Name;
        // For DP, the input size is 'n' itself (e.g., calculating 50th fib number).
        // Since the prompt asks for "Input Size: 100, 10000...", calculating Fib(10000) will overflow long easily.
        // Also Fib(10000) is huge.
        // We will adapt:
        // If input is large (like an array), we might just take the length as 'N'.
        // But Fib(10000) is instantaneous with DP (O(N)).
        // To make it benchmarkable, maybe we do it N times? Or we calculate Fib(N % 90) to avoid overflow but do the loop N times?
        // Or we stick to the prompt's "Input Size" meaning N.
        // Calculating Fib(10000) using BigInteger would be slow enough.
        // Let's use Iterative DP with long, but limit N to 92 (max long).
        // If Logic: If input is int N, we calculate Fib(min(N, 92)).
        // Actually, to make it consume time for "Large (1,000,000)", maybe we should calculate Fib(N) using BigInteger?
        // Let's just calculate Fib(N) with BigInteger if possible, or just loop to N.
        // Let's just do a simple O(N) loop that simulates work if we just want to measure 'N' complexity.
        // A simple loop summing things up to N is O(N).
        // Let's implement actual Fib, but modulo separate, or use BigInteger?
        // For standard "Fibonacci DP", usually we compute F(n). 
        // Let's interpret 'input' as integer N.

        public void Execute(object input)
        {
            int n = 0;
            if (input is int val) n = val;
            else if (input is int[] arr) n = arr.Length; // Fallback if runner sends array
            else throw new ArgumentException("Invalid input for Fibonacci");

            // To avoid overflow and make it scale with N, let's just compute F(n) modulo M
            // This is O(N)
            CalculateFibonacci(n);
        }

        private long CalculateFibonacci(int n)
        {
            if (n <= 1) return n;

            long[] dp = new long[n + 1];
            dp[0] = 0;
            dp[1] = 1;

            for (int i = 2; i <= n; i++)
            {
                // Just add, allow overflow (it wraps around, still O(N) operations) 
                dp[i] = dp[i - 1] + dp[i - 2];
            }

            return dp[n];
        }
    }
}
