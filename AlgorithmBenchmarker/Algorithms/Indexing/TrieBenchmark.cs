using System;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
        public bool IsEndOfWord;
    }

    public class TrieBenchmark : IAlgorithm
    {
        public string Name => "Trie Insert/Search";
        public string Category => "Indexing";
        public string Complexity => "O(L)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is string[] words)
            {
                var root = new TrieNode();
                
                // Benchmark: Insert all
                foreach (var word in words)
                {
                    Insert(root, word);
                }

                // And Search all (to make it a complete usage benchmark)
                foreach(var word in words)
                {
                    Search(root, word);
                }
            }
        }

        private void Insert(TrieNode root, string word)
        {
            var node = root;
            foreach (var ch in word)
            {
                if (!node.Children.ContainsKey(ch))
                    node.Children[ch] = new TrieNode();
                node = node.Children[ch];
            }
            node.IsEndOfWord = true;
        }

        private bool Search(TrieNode root, string word)
        {
            var node = root;
            foreach (var ch in word)
            {
                if (!node.Children.ContainsKey(ch)) return false;
                node = node.Children[ch];
            }
            return node.IsEndOfWord;
        }
    }
}
