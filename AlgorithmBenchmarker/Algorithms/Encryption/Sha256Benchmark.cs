using System;
using System.Security.Cryptography;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Encryption
{
    public class Sha256Benchmark : IAlgorithm
    {
        public string Name => "SHA256 Hash";
        public string Category => "Encryption";
        public string Complexity => "O(N)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is int size)
            {
                byte[] data = new byte[size];
                // Random data generation cost is included here?
                // Ideally input is pre-gen. But AesBenchmark uses generated string.
                // We keep consistency: Encryption category = generate locally.
                // To minimize alloc noise, generic new Random is fast enough (or just 0s).
                // Let's use 0s to focus on Hashing speed.
                
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(data);
                }
            }
        }
    }
}
