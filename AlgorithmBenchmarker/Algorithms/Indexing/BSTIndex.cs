using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.Indexing
{
    public class BSTIndex : IAlgorithm
    {
        public string Name => "Binary Search Tree Index";
        public string Category => "Indexing";
        public string Complexity => "O(log N)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is IndexingInputData data)
            {
                TreeNode root = null;
                // Build
                foreach (var val in data.Dataset) root = Insert(root, val);

                // Query
                foreach (var query in data.SearchQueries)
                {
                    bool found = Search(root, query);
                }
            }
        }

        private class TreeNode
        {
            public int val;
            public TreeNode left, right;
            public TreeNode(int val) { this.val = val; }
        }

        private TreeNode Insert(TreeNode root, int val)
        {
            if (root == null) return new TreeNode(val);
            if (val < root.val) root.left = Insert(root.left, val);
            else if (val > root.val) root.right = Insert(root.right, val);
            return root;
        }

        private bool Search(TreeNode root, int val)
        {
            if (root == null) return false;
            if (root.val == val) return true;
            return val < root.val ? Search(root.left, val) : Search(root.right, val);
        }
    }
}
