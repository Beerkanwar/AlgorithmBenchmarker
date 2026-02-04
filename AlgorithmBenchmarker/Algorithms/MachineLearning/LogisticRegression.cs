using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class LogisticRegression : IAlgorithm
    {
        public string Name => "Logistic Regression Training";
        public string Category => "Machine Learning";
        public string Complexity => "O(N*D*Epochs)";

        public void Execute(object input)
        {
            if (input is MLInputData data)
            {
                int samples = data.Features.Length;
                if (samples == 0) return;
                int features = data.Features[0].Length;

                double[] weights = new double[features];
                double bias = 0;
                double lr = 0.01;
                int epochs = BenchmarkContext.Current?.Epochs ?? 10;

                for (int e = 0; e < epochs; e++)
                {
                    for (int i = 0; i < samples; i++)
                    {
                        double linear = bias;
                        for (int j = 0; j < features; j++) linear += weights[j] * data.Features[i][j];
                        
                        double prediction = 1.0 / (1.0 + Math.Exp(-linear));
                        
                        // Convert regression label to 0/1 approx
                        double target = data.Labels[i] > 0.5 ? 1.0 : 0.0;
                        
                        double error = prediction - target;
                        bias -= lr * error;
                        for (int j = 0; j < features; j++)
                            weights[j] -= lr * error * data.Features[i][j];
                    }
                }
            }
        }
    }
}
