using System;
using AlgorithmBenchmarker.Models;
using System.Globalization;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class TrieSearch : IAlgorithm
    {
        public string Name => "Trie Prefix Search";
        public string Category => "Indexing";
        public string Complexity => "O(L)";

        public void Execute(object input)
        {
            if (input is IndexingInputData data)
            {
                // IndexingInputData provides int[].
                // Trie usually works on Strings.
                // Convert ints to strings.
                
                var root = new TrieNode();
                
                foreach (var val in data.Dataset)
                {
                    Insert(root, val.ToString(CultureInfo.InvariantCulture));
                }

                foreach (var query in data.SearchQueries)
                {
                    bool found = Search(root, query.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private class TrieNode
        {
             // 0-9 digits only
            public TrieNode[] children = new TrieNode[10];
            public bool isEndOfWord;
        }

        private void Insert(TrieNode root, string key)
        {
            TrieNode pCrawl = root;
            foreach(char c in key)
            {
                int index = c - '0';
                if (index < 0 || index > 9) continue; // Safety
                if (pCrawl.children[index] == null)
                    pCrawl.children[index] = new TrieNode();
                pCrawl = pCrawl.children[index];
            }
            pCrawl.isEndOfWord = true;
        }

        private bool Search(TrieNode root, string key)
        {
            TrieNode pCrawl = root;
            foreach(char c in key)
            {
                int index = c - '0';
                if (index < 0 || index > 9) return false;
                if (pCrawl.children[index] == null) return false;
                pCrawl = pCrawl.children[index];
            }
            return (pCrawl != null && pCrawl.isEndOfWord);
        }
    }
}
