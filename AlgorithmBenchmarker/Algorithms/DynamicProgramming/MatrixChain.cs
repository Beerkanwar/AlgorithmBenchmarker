using System;

namespace AlgorithmBenchmarker.Algorithms.DynamicProgramming
{
    public class MatrixChain : IAlgorithm
    {
        public string Name => "Matrix Chain Multiplication";
        public string Category => "Dynamic Programming";
        public string Complexity => "O(N^3)";

        public void Execute(object input)
        {
            if (input is int n)
            {
                // N matrices => dimensions array size N+1
                // Limit N because O(N^3) explodes fast. 
                // BenchmarkRunner passes 100, 200, 1000...
                // 1000^3 = 1 Billion. Might take seconds. OK.
                
                int[] p = new int[n + 1];
                var rnd = new Random(42);
                for(int i=0; i<=n; i++) p[i] = rnd.Next(10, 100);

                int[,] m = new int[n + 1, n + 1];
                
                for (int i = 1; i <= n; i++) m[i, i] = 0;

                for (int L = 2; L <= n; L++)
                {
                    for (int i = 1; i <= n - L + 1; i++)
                    {
                        int j = i + L - 1;
                        m[i, j] = int.MaxValue;
                        for (int k = i; k <= j - 1; k++)
                        {
                            int q = m[i, k] + m[k + 1, j] + p[i - 1] * p[k] * p[j];
                            if (q < m[i, j]) m[i, j] = q;
                        }
                    }
                }
            }
        }
    }
}
