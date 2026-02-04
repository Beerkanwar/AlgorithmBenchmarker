using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class KMeans : IAlgorithm
    {
        public string Name => "K-Means Clustering";
        public string Category => "Machine Learning";
        public string Complexity => "O(N*K*Epochs)";

        public void Execute(object input)
        {
            if (input is MLInputData data)
            {
                int samples = data.Features.Length;
                if (samples == 0) return;
                int features = data.Features[0].Length;
                
                int K = 3; // Fixed K or from Config? Config doesn't have K. Using 3.
                int epochs = BenchmarkContext.Current?.Epochs ?? 10;
                
                // Init centroids
                double[][] centroids = new double[K][];
                for(int k=0; k<K; k++)
                {
                    centroids[k] = new double[features];
                    Array.Copy(data.Features[k % samples], centroids[k], features);
                }
                
                int[] assignments = new int[samples];
                
                for (int e = 0; e < epochs; e++)
                {
                    // Assignment step
                    for (int i = 0; i < samples; i++)
                    {
                        double minDist = double.MaxValue;
                        int bestK = 0;
                        for (int k = 0; k < K; k++)
                        {
                            double dist = Distance(data.Features[i], centroids[k]);
                            if (dist < minDist) { minDist = dist; bestK = k; }
                        }
                        assignments[i] = bestK;
                    }
                    
                    // Update step
                    double[][] newCentroids = new double[K][];
                    int[] counts = new int[K];
                     for(int k=0; k<K; k++) newCentroids[k] = new double[features];
                    
                    for (int i = 0; i < samples; i++)
                    {
                        int cluster = assignments[i];
                        counts[cluster]++;
                        for (int j = 0; j < features; j++)
                             newCentroids[cluster][j] += data.Features[i][j];
                    }
                    
                    for(int k=0; k<K; k++)
                    {
                        if (counts[k] > 0)
                            for (int j = 0; j < features; j++) centroids[k][j] = newCentroids[k][j] / counts[k];
                    }
                }
            }
        }

        private double Distance(double[] a, double[] b)
        {
            double sum = 0;
            for(int i=0; i<a.Length; i++) sum += (a[i]-b[i])*(a[i]-b[i]);
            return sum;
        }
    }
}
