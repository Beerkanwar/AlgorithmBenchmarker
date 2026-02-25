using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmBenchmarker.Algorithms.Compression
{
    public class RleBenchmark : IAlgorithm
    {
        public string Name => "Run-Length Encoding (RLE)";
        public string Category => "Compression";
        public string Complexity => "O(N)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is string[] sArr)
            {
                foreach (var s in sArr) EncodeString(s);
            }
            else if (input is byte[] bArr)
            {
                EncodeBytes(bArr);
            }
        }

        private void EncodeString(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var sb = new StringBuilder();
            int n = text.Length;
            for (int i = 0; i < n; i++)
            {
                int count = 1;
                while (i < n - 1 && text[i] == text[i + 1])
                {
                    count++;
                    i++;
                }
                sb.Append(text[i]);
                sb.Append(count);
            }
        }

        private void EncodeBytes(byte[] data)
        {
            if (data.Length == 0) return;
            var result = new List<byte>();
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                byte val = data[i];
                byte count = 1;
                while (i < n - 1 && data[i+1] == val && count < 255)
                {
                    count++;
                    i++;
                }
                result.Add(val);
                result.Add(count);
            }
        }
    }
}
