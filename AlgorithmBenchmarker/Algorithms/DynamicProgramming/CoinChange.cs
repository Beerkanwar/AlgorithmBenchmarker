using System;

namespace AlgorithmBenchmarker.Algorithms.DynamicProgramming
{
    public class CoinChange : IAlgorithm
    {
        public string Name => "Coin Change Problem";
        public string Category => "Dynamic Programming";
        public string Complexity => "O(N*Target)";

        public void Execute(object input)
        {
            if (input is int n)
            {
                // N represents Target Amount for benchmark scaling
                // Coins are constant set or few randoms
                int[] coins = { 1, 2, 5, 10, 20, 50, 100 };
                int V = n;
                
                int[] table = new int[V + 1];
                table[0] = 0;
                for (int i = 1; i <= V; i++) table[i] = int.MaxValue;

                for (int i = 1; i <= V; i++)
                {
                    for (int j = 0; j < coins.Length; j++)
                    {
                        if (coins[j] <= i)
                        {
                            int sub_res = table[i - coins[j]];
                            if (sub_res != int.MaxValue && sub_res + 1 < table[i])
                                table[i] = sub_res + 1;
                        }
                    }
                }
            }
        }
    }
}
