using System;

namespace AlgorithmBenchmarker.Algorithms.DynamicProgramming
{
    public class LCS : IAlgorithm
    {
        public string Name => "Longest Common Subsequence";
        public string Category => "Dynamic Programming";
        public string Complexity => "O(N*M)";

        public void Execute(object input)
        {
            if (input is int n)
            {
                // Generate two strings length N
                string X = new string('A', n); // Dummy content doesn't affect speed logic much
                string Y = new string('B', n); 
                // Mix it up slightly so it's not trivial
                X = GenerateRandomString(n, 42);
                Y = GenerateRandomString(n, 100);

                int m = Y.Length;
                int[,] L = new int[n + 1, m + 1];

                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= m; j++)
                    {
                        if (i == 0 || j == 0)
                            L[i, j] = 0;
                        else if (X[i - 1] == Y[j - 1])
                            L[i, j] = L[i - 1, j - 1] + 1;
                        else
                            L[i, j] = Math.Max(L[i - 1, j], L[i, j - 1]);
                    }
                }
            }
        }

        private string GenerateRandomString(int len, int seed)
        {
            var rnd = new Random(seed);
            char[] chars = new char[len];
            for(int i=0; i<len; i++) chars[i] = (char)('A' + rnd.Next(0, 26));
            return new string(chars);
        }
    }
}
