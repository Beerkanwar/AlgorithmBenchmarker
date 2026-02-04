using System;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Algorithms.MachineLearning
{
    public class Perceptron : IAlgorithm
    {
        public string Name => "Perceptron Classifier";
        public string Category => "Machine Learning";
        public string Complexity => "O(N*Epochs)";

        public void Execute(object input)
        {
            if (input is MLInputData data)
            {
                int samples = data.Features.Length;
                if (samples == 0) return;
                int features = data.Features[0].Length;

                double[] weights = new double[features];
                double bias = 0;
                double lr = 0.1;
                int epochs = BenchmarkContext.Current?.Epochs ?? 10;

                for (int e = 0; e < epochs; e++)
                {
                    for (int i = 0; i < samples; i++)
                    {
                        double sum = bias;
                        for (int j = 0; j < features; j++) sum += weights[j] * data.Features[i][j];
                        
                        double output = sum > 0 ? 1 : 0;
                        double target = data.Labels[i] > 0.5 ? 1 : 0;
                        double error = target - output;
                        
                        if (error != 0)
                        {
                            bias += lr * error;
                            for (int j = 0; j < features; j++)
                                weights[j] += lr * error * data.Features[i][j];
                        }
                    }
                }
            }
        }
    }
}
