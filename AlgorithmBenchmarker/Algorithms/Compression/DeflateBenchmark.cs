using System;
using System.IO;
using System.IO.Compression;

namespace AlgorithmBenchmarker.Algorithms.Compression
{
    public class DeflateBenchmark : IAlgorithm
    {
        public string Name => "Deflate Compression";
        public string Category => "Compression";
        public string Complexity => "O(N)";

        public override string ToString() => Name;

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
                using (DeflateStream deflate = new DeflateStream(ms, CompressionLevel.Optimal))
                using (StreamWriter sw = new StreamWriter(deflate))
                {
                    sw.Write(text);
                }
            }
        }

        private void CompressBytes(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(ms, CompressionLevel.Optimal))
                {
                    deflate.Write(data, 0, data.Length);
                }
            }
        }
    }
}
