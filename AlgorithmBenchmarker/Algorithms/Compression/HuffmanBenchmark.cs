using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmBenchmarker.Algorithms.Compression
{
    public class HuffmanBenchmark : IAlgorithm
    {
        public string Name => "Huffman Coding";
        public string Category => "Compression";
        public string Complexity => "O(N log N)";

        public void Execute(object input)
        {
            if (input is string[] sArr)
            {
                foreach (var s in sArr) Encode(s);
            }
             else if (input is byte[] bArr)
            {
                 // Convert bytes to 'string' representation for simplicity of Huffman node logic
                 // Or implement Byte-based Huffman.
                 // For benchmark, let's treat bytes as chars.
                 // Byte based is better.
                 EncodeBytes(bArr);
            }
        }

        private void Encode(string text)
        {
            var freqs = new Dictionary<char, int>();
            foreach (char c in text)
            {
                if (!freqs.ContainsKey(c)) freqs[c] = 0;
                freqs[c]++;
            }
            BuildTree(freqs.ToDictionary(k => (object)k.Key, k => k.Value));
        }

        private void EncodeBytes(byte[] data)
        {
            var freqs = new Dictionary<byte, int>();
            foreach (byte b in data)
            {
                if (!freqs.ContainsKey(b)) freqs[b] = 0;
                freqs[b]++;
            }
             BuildTree(freqs.ToDictionary(k => (object)k.Key, k => k.Value));
        }

        private void BuildTree(Dictionary<object, int> freqs)
        {
            // Priority Queue (SortedList/MinHeap). Using List and Sorting for Simplicity O(K log K)
            var nodes = freqs.Select(kv => new HuffmanNode { Symbol = kv.Key, Frequency = kv.Value }).ToList();

            while (nodes.Count > 1)
            {
                nodes.Sort((a, b) => a.Frequency.CompareTo(b.Frequency));
                
                var left = nodes[0];
                var right = nodes[1];
                nodes.RemoveRange(0, 2);

                var parent = new HuffmanNode
                {
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                nodes.Add(parent);
            }
            
            // Generate Codes (Traversal)
            if (nodes.Count > 0) Traverse(nodes[0], "");
        }

        private void Traverse(HuffmanNode node, string code)
        {
            if (node.Left == null && node.Right == null)
            {
                // Leaf
                return;
            }
            if (node.Left != null) Traverse(node.Left, code + "0");
            if (node.Right != null) Traverse(node.Right, code + "1");
        }

        private class HuffmanNode
        {
            public object Symbol;
            public int Frequency;
            public HuffmanNode Left;
            public HuffmanNode Right;
        }
    }
}
