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

        // Changed return type to List<BenchmarkResult> for range support
        public List<BenchmarkResult> RunBatch(IAlgorithm algorithm, BenchmarkConfig config)
        {
            var results = new List<BenchmarkResult>();
            string batchId = Guid.NewGuid().ToString();

            // Validate Range
            if (config.MinInputSize > config.MaxInputSize) 
                throw new ArgumentException("Min Size cannot be greater than Max Size");
            if (config.StepSize <= 0) 
                config.StepSize = 1;

            for (int size = config.MinInputSize; size <= config.MaxInputSize; size += config.StepSize)
            {
                // Temporarily override InputSize in a config clone or just pass size
                // We'll trust GenerateInput uses 'size'
                
                // We need to modify the config or method to accept specific size
                // InputGenerator takes 'config', so let's modify a temp config or overload GenerateInput.
                // Cleaner: Pass 'size' explicitly to GenerateInput? 
                // Currently InputGenerator.GenerateInput uses config.InputSize.
                // Let's mutate config temporarily (thread safe only if single threaded).
                // Or better: Create a focused config for this iteration.
                
                var iterationConfig = new BenchmarkConfig 
                { 
                     InputSize = size,
                     InputType = config.InputType,
                     Distribution = config.Distribution,
                     Repetitions = config.Repetitions
                     // copy others if needed
                };

                var result = RunSingle(algorithm, iterationConfig, batchId);
                results.Add(result);
            }

            return results;
        }

        private BenchmarkResult RunSingle(IAlgorithm algorithm, BenchmarkConfig config, string batchId)
        {
            var masterInput = _inputGenerator.GenerateInput(config, algorithm.Category); // Uses config.InputSize

            // Warmup
            object warmupInput = CloneInput(masterInput);
            try { algorithm.Execute(warmupInput); } catch { /* ignore warmup errors */ }

            double totalTimeMs = 0;
            long totalMemoryDelta = 0;

            for (int i = 0; i < config.Repetitions; i++)
            {
                object input = CloneInput(masterInput);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                long memStart = GC.GetTotalMemory(true);
                Stopwatch sw = Stopwatch.StartNew();

                algorithm.Execute(input);

                sw.Stop();
                long memEnd = GC.GetTotalMemory(false);

                totalTimeMs += sw.Elapsed.TotalMilliseconds;
                long delta = Math.Max(0, memEnd - memStart);
                totalMemoryDelta += delta;
            }

            return new BenchmarkResult
            {
                BatchId = batchId,
                AlgorithmName = algorithm.Name,
                Category = algorithm.Category,
                InputSize = config.InputSize,
                AvgTimeMs = totalTimeMs / config.Repetitions,
                MemoryBytes = totalMemoryDelta / config.Repetitions,
                Timestamp = DateTime.Now
            };
        }

        private object CloneInput(object input)
        {
            if (input is int[] iArr) return (int[])iArr.Clone();
            if (input is float[] fArr) return (float[])fArr.Clone();
            if (input is string[] sArr) return (string[])sArr.Clone();
            
            // New cloning logic might be needed for new types (Matrix, Graph)
            // GraphData we implemented earlier doesn't support deep clone yet.
            // If BFS doesn't modify graph, we are fine. 
            // BFS uses `bool[] visited` internal to the algo, doesn't touch graph. OK.
            
            return input;
        }
    }
}
