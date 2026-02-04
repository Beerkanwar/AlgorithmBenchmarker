using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class LinearRegression : IAlgorithm
    {
        public string Name => "Linear Regression Training";
        public string Category => "Machine Learning";
        public string Complexity => "O(N*D*Epochs)";

        public void Execute(object input)
        {
            if (input is MLInputData data)
            {
                int samples = data.Features.Length;
                if (samples == 0) return;
                int features = data.Features[0].Length;
                
                // Weights
                double[] weights = new double[features];
                double bias = 0;
                double lr = 0.01;
                
                int epochs = BenchmarkContext.Current?.Epochs ?? 10;
                
                for (int e = 0; e < epochs; e++)
                {
                    // Gradient Descent (Stochastic or Batch?)
                    // Simple SGD for typical benchmark load
                    for (int i = 0; i < samples; i++)
                    {
                        double prediction = bias;
                        for (int j = 0; j < features; j++) prediction += weights[j] * data.Features[i][j];
                        
                        double error = prediction - data.Labels[i];
                        
                        bias -= lr * error;
                        for (int j = 0; j < features; j++)
                            weights[j] -= lr * error * data.Features[i][j];
                    }
                }
            }
        }
    }
}
