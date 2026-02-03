using System;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class MatrixMultBenchmark : IAlgorithm
    {
        public string Name => "Matrix Multiplication";
        public string Category => "ML Optimization";

        public void Execute(object input)
        {
            // InputSize N -> NxN matrix
            // InputGenerator returns int/string/graph. 
            // If we selected "ML Optimization", InputGenerator needs to give us N or Matrix.
            // If InputGenerator gives us an int N (from "Custom input" logic in DP), we use that.
            
            // We need to update InputGenerator to support returning simple N for this category too.
            
            int n = 0;
            if (input is int val) n = val;
            else if (input is string[] sArr) n = sArr.Length; // Fallback
            else if (input is int[] iArr) n = iArr.Length;
            
            // Limit N? Matrix Mult is O(N^3). N=1000 is 1 billion ops. Might be slow but OK for benchmark (seconds).
            // N=10,000 is too big. 
            // User responsibility to choose reasonable N.
            
            // Generate dummy matrices
            double[,] A = new double[n, n];
            double[,] B = new double[n, n];
            
            // Fill deterministic
            for(int i=0; i<n; i++)
                for(int j=0; j<n; j++) { A[i,j] = 1.0; B[i,j] = 2.0; }

            Multiply(A, B, n);
        }

        private double[,] Multiply(double[,] A, double[,] B, int n)
        {
            var C = new double[n, n];
            // Naive O(N^3)
            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        C[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return C;
        }
    }
}
