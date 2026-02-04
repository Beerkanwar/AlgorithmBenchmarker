using System;

namespace AlgorithmBenchmarker.Algorithms.DynamicProgramming
{
    public class Knapsack : IAlgorithm
    {
        public string Name => "0/1 Knapsack";
        public string Category => "Dynamic Programming";
        public string Complexity => "O(N*W)";

        public void Execute(object input)
        {
            if (input is int n)
            {
                // Generate N items
                int[] val = new int[n];
                int[] wt = new int[n];
                var rnd = new Random(42);
                for(int i=0; i<n; i++) { val[i] = rnd.Next(1, 100); wt[i] = rnd.Next(1, 20); }
                
                int W = n * 5; // Capacity related to N
                
                int[,] K = new int[n + 1, W + 1];

                for (int i = 0; i <= n; i++)
                {
                    for (int w = 0; w <= W; w++)
                    {
                        if (i == 0 || w == 0)
                            K[i, w] = 0;
                        else if (wt[i - 1] <= w)
                            K[i, w] = Math.Max(val[i - 1] + K[i - 1, w - wt[i - 1]], K[i - 1, w]);
                        else
                            K[i, w] = K[i - 1, w];
                    }
                }
            }
        }
    }
}
