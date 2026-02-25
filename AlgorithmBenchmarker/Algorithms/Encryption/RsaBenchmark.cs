using System;
using System.Security.Cryptography;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Encryption
{
    public class RsaBenchmark : IAlgorithm
    {
        public string Name => "RSA Encryption";
        public string Category => "Encryption";
        public string Complexity => "O(N^2)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is int size)
            {
                // RSA Key Generation is expensive, usually done outside loop, 
                // but benchmarking usually includes it if we want full crypto cost, 
                // OR we generate once. 
                // BenchmarkRunner runs 'Execute' repeatedly.
                // Re-generating RSA (2048) 5 times is VERY slow.
                // But if we reuse, we are only benchmarking Encrypt.
                // Let's generate Key based on Config (e.g. 1024, 2048).
                
                int keySize = BenchmarkContext.Current?.KeySize ?? 2048;
                if (keySize < 512) keySize = 512; // Min valid

                using (RSA rsa = RSA.Create(keySize))
                {
                    // Payload limited for RSA (KeySize/8 - padding).
                    // If Size > Max, we must chop.
                    // Benchmark simplifies to encrypting ONE block of Size (capped).
                    
                    int maxBlock = (keySize / 8) - 42; 
                    if (maxBlock <= 0) maxBlock = 10;
                    
                    int actualSize = Math.Min(size, maxBlock);
                    byte[] data = new byte[actualSize];
                    new Random().NextBytes(data);
                    
                    byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                }
            }
        }
    }
}
