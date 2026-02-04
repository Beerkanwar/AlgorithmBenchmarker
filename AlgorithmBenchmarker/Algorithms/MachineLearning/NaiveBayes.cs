using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class NaiveBayes : IAlgorithm
    {
        public string Name => "Naive Bayes Classifier";
        public string Category => "Machine Learning";
        public string Complexity => "O(N*D)";

        public void Execute(object input)
        {
            if (input is MLInputData data)
            {
                // Gaussian Naive Bayes Training
                // Calculate Mean and Variance per feature per class
                // Assume 2 classes (0, 1) based on regression label split
                
                int samples = data.Features.Length;
                if (samples == 0) return;
                int features = data.Features[0].Length;

                double[,] means = new double[2, features];
                double[,] vars = new double[2, features];
                int[] counts = new int[2];

                for (int i = 0; i < samples; i++)
                {
                    int c = data.Labels[i] > 0.5 ? 1 : 0;
                    counts[c]++;
                    for(int j=0; j<features; j++) means[c, j] += data.Features[i][j];
                }

                for (int c = 0; c < 2; c++)
                {
                    if (counts[c] == 0) continue;
                    for (int j = 0; j < features; j++) means[c, j] /= counts[c];
                }

                for (int i = 0; i < samples; i++)
                {
                    int c = data.Labels[i] > 0.5 ? 1 : 0;
                    for (int j = 0; j < features; j++)
                    {
                        double diff = data.Features[i][j] - means[c, j];
                        vars[c, j] += diff * diff;
                    }
                }
                
                 for (int c = 0; c < 2; c++)
                {
                    if (counts[c] == 0) continue;
                    for (int j = 0; j < features; j++) vars[c, j] /= counts[c];
                }
            }
        }
    }
}
