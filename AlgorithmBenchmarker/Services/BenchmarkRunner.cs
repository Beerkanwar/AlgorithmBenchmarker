using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Services
{
    public class BenchmarkRunner
    {
        private readonly InputGenerator _inputGenerator;

        public BenchmarkRunner()
        {
            _inputGenerator = new InputGenerator();
        }

        public List<BenchmarkResult> RunBatch(IAlgorithm algorithm, BenchmarkConfig config, CancellationToken token, IProgress<int> progress)
        {
            var results = new List<BenchmarkResult>();
            string batchId = Guid.NewGuid().ToString();

            if (config.MinInputSize > config.MaxInputSize) 
                throw new ArgumentException("Min Size cannot be greater than Max Size");
            if (config.StepSize <= 0) 
                config.StepSize = 1;

            int totalSteps = (config.MaxInputSize - config.MinInputSize) / config.StepSize + 1;
            int currentStep = 0;

            for (int size = config.MinInputSize; size <= config.MaxInputSize; size += config.StepSize)
            {
                if (token.IsCancellationRequested) break;

                // Create a config specific for this iteration
                var iterationConfig = new BenchmarkConfig 
                { 
                     InputSize = size,
                     InputType = config.InputType,
                     Distribution = config.Distribution,
                     Repetitions = config.Repetitions,
                     // Copy other props
                     KeySize = config.KeySize,
                     CipherMode = config.CipherMode,
                     BlockSize = config.BlockSize,
                     CompressionLevel = config.CompressionLevel,
                     CompressionInputType = config.CompressionInputType,
                     EnforceSortedInput = config.EnforceSortedInput,
                     TargetPosition = config.TargetPosition,
                     UseMemoization = config.UseMemoization,
                     GraphDensity = config.GraphDensity,
                     IsDirected = config.IsDirected,
                     IsWeighted = config.IsWeighted,
                     QueryCount = config.QueryCount,
                     KeyDistribution = config.KeyDistribution,
                     FeatureDimension = config.FeatureDimension,
                     Epochs = config.Epochs,
                     BatchSize = config.BatchSize,
                     CostMetric = config.CostMetric
                };

                var result = RunSingle(algorithm, iterationConfig, batchId, token);
                if (result != null) results.Add(result);

                currentStep++;
                progress?.Report((int)((double)currentStep / totalSteps * 100));
            }

            return results;
        }

        private BenchmarkResult? RunSingle(IAlgorithm algorithm, BenchmarkConfig config, string batchId, CancellationToken token)
        {
            var masterInput = _inputGenerator.GenerateInput(config, algorithm.Category);

            // 1. Warmup
            try 
            {
                object warmupInput = CloneInput(masterInput);
                algorithm.Execute(warmupInput);
                
                // Force Clean
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            } 
            catch { /* ignore warmup errors */ }

            List<double> executionTimes = new List<double>();
            long totalAllocated = 0;

            for (int i = 0; i < config.Repetitions; i++)
            {
                if (token.IsCancellationRequested) return null;

                object input = CloneInput(masterInput);

                // Clear memory before run to minimize noise (though AllocatedBytesForCurrentThread is robust)
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                long beforeBytes = GC.GetAllocatedBytesForCurrentThread();
                Stopwatch sw = Stopwatch.StartNew();

                algorithm.Execute(input);

                sw.Stop();
                long afterBytes = GC.GetAllocatedBytesForCurrentThread();

                executionTimes.Add(sw.Elapsed.TotalMilliseconds);
                totalAllocated += (afterBytes - beforeBytes);
            }

            if (executionTimes.Count == 0) return null;

            double avgTime = executionTimes.Average();
            double minTime = executionTimes.Min();
            double maxTime = executionTimes.Max();
            double stdDev = CalculateStdDev(executionTimes, avgTime);

            return new BenchmarkResult
            {
                BatchId = batchId,
                AlgorithmName = algorithm.Name,
                Category = algorithm.Category,
                InputSize = config.InputSize,
                AvgTimeMs = avgTime,
                MinTimeMs = minTime,
                MaxTimeMs = maxTime,
                StdDevTimeMs = stdDev,
                AllocatedBytes = totalAllocated / config.Repetitions,
                MemoryBytes = totalAllocated / config.Repetitions, // Mapping to old field too just in case
                Timestamp = DateTime.Now
            };
        }

        private double CalculateStdDev(List<double> values, double avg)
        {
            if (values.Count <= 1) return 0;
            double sumOfSquares = 0;
            foreach(var val in values) sumOfSquares += Math.Pow(val - avg, 2);
            return Math.Sqrt(sumOfSquares / (values.Count - 1));
        }

        private object CloneInput(object input)
        {
            if (input is int[] iArr) return (int[])iArr.Clone();
            if (input is float[] fArr) return (float[])fArr.Clone();
            if (input is string[] sArr) return (string[])sArr.Clone();
            return input;
        }
    }
}
