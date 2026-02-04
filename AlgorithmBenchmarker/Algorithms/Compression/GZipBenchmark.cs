using System;
using System.IO;
using System.IO.Compression;

namespace AlgorithmBenchmarker.Algorithms.Compression
{
    public class GZipBenchmark : IAlgorithm
    {
        public string Name => "GZip Compression";
        public string Category => "Compression";
        public string Complexity => "O(N)";

        public void Execute(object input)
        {
            if (input is string[] sArr)
            {
                foreach (var s in sArr) Compress(s);
            }
        }

        private void Compress(string text)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionLevel.Fastest))
                using (StreamWriter sw = new StreamWriter(gzip))
                {
                    sw.Write(text);
                }
            }
        }
    }
}
