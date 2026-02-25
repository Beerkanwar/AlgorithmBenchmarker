using System;
using System.IO;
using System.Security.Cryptography;

namespace AlgorithmBenchmarker.Algorithms.Encryption
{
    public class AesBenchmark : IAlgorithm
    {
        public string Name => "AES Encryption";
        public string Category => "Encryption";
        public string Complexity => "O(N)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            // Input should be a byte array ideally, keeping it simple with string input generated
            // If InputGenerator sends string[], we can concat? Or just use one string?
            // InputGenerator generates string[] for Strings. 
            // We can also support int[] and just convert to bytes.
            
            byte[] data;
            
            if (input is string[] sArr)
            {
                 // Join them to simulate a large text block
                 // Or just pick one? 
                 // To scale with "InputSize", config.InputSize determined the array length.
                 // So we have 'Size' strings. Let's encrypt all of them.
                 // Or just assume InputGenerator creates a byte array for Encryption?
                 // Let's adapt.
                 
                 // If string[], encrypt each.
                 foreach(var s in sArr) Encrypt(s);
                 return;
            }
            
            // If we receive other types, do nothing or throw.
            // But let's handle "int[]" as raw bytes for fun? 
            // Or just check InputGenerator.

            // Assume InputGenerator might be updated or we use existing.
            // Let's default to generating a dummy payload of size N if input is int (count).
            
            if (input is int count)
            {
                data = new byte[count];
                EncryptBytes(data);
            }
        }

        private void Encrypt(string text)
        {
            // Simple AES setup
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                    }
                }
            }
        }

        private void EncryptBytes(byte[] data)
        {
             using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
            }
        }
    }
}
