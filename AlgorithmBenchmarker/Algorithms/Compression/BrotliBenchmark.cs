using System;
using System.IO;
using System.IO.Compression;

namespace AlgorithmBenchmarker.Algorithms.Compression
{
    public class BrotliBenchmark : IAlgorithm
    {
        public string Name => "Brotli Compression";
        public string Category => "Compression";
        public string Complexity => "O(N)";

        public void Execute(object input)
        {
            if (input is string[] sArr)
            {
                foreach (var s in sArr) CompressString(s);
            }
            else if (input is byte[] bArr)
            {
                CompressBytes(bArr);
            }
        }

        private void CompressString(string text)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BrotliStream brotli = new BrotliStream(ms, CompressionLevel.Optimal))
                using (StreamWriter sw = new StreamWriter(brotli))
                {
                    sw.Write(text);
                }
            }
        }

        private void CompressBytes(byte[] data)
        {
             using (MemoryStream ms = new MemoryStream())
            {
                using (BrotliStream brotli = new BrotliStream(ms, CompressionLevel.Optimal))
                {
                    brotli.Write(data, 0, data.Length);
                }
            }
        }
    }
}
