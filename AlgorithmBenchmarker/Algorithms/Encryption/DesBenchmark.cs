using System;
using System.IO;
using System.Security.Cryptography;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Encryption
{
    // Proxy for Blowfish (Legacy Symmetric Block Cipher)
    public class DesBenchmark : IAlgorithm
    {
        public string Name => "DES (Legacy)";
        public string Category => "Encryption";
        public string Complexity => "O(N)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is int size)
            {
                byte[] data = new byte[size];
                
                using (DES des = DES.Create())
                {
                    des.GenerateKey();
                    des.GenerateIV();
                    
                    using (ICryptoTransform encryptor = des.CreateEncryptor(des.Key, des.IV))
                    using (MemoryStream ms = new MemoryStream())
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                }
            }
        }
    }
}
