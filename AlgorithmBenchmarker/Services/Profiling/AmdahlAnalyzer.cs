using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class ThreadScalingResult
    {
        public int ThreadCount { get; set; }
        public double ExecutionTimeMs { get; set; }
        public double Speedup { get; set; }
        public double Efficiency { get; set; }
        public double EstimatedSerialFraction { get; set; }
    }

    /// <summary>
    /// Concurrency & Thread Scaling Profiler (Amdahl Analyzer).
    /// Orchestrates parallel benchmarking and computes theoretical scaling bounds.
    /// </summary>
    public class AmdahlAnalyzer
    {
        public List<ThreadScalingResult> AnalyzeScaling(IAlgorithm algorithm, object baseInput, int maxThreads)
        {
            var results = new List<ThreadScalingResult>();
            double baseTime = 0;

            for (int t = 1; t <= maxThreads; t++)
            {
                // Force GC for clean state
                GC.Collect();
                GC.WaitForPendingFinalizers();

                double totalTime = RunParallel(algorithm, baseInput, t);

                if (t == 1) baseTime = totalTime;

                double speedup = baseTime / totalTime;
                double efficiency = speedup / t;
                
                // Amdahl's Law: Speedup = 1 / ((1 - P) + P/N)
                // 1/S = (1-P) + P/N => 1/S = 1 - P(1 - 1/N) => P = (1 - 1/S) / (1 - 1/N)
                // P is parallel fraction. Substituted, (1-P) is Serial Fraction.
                double parallelFraction = t > 1 ? (1.0 - (1.0 / speedup)) / (1.0 - (1.0 / t)) : 0;
                double serialFraction = t > 1 ? 1.0 - parallelFraction : 1.0;

                results.Add(new ThreadScalingResult
                {
                    ThreadCount = t,
                    ExecutionTimeMs = totalTime,
                    Speedup = speedup,
                    Efficiency = efficiency,
                    EstimatedSerialFraction = serialFraction < 0 ? 0 : serialFraction // Clamp bounds due to noise
                });
            }

            return results;
        }

        private double RunParallel(IAlgorithm algorithm, object input, int threadCount)
        {
            var tasks = new Task[threadCount];
            var sw = new Stopwatch();

            // Barrier to synchronize start times explicitly
            using (var barrier = new Barrier(threadCount + 1))
            {
                for (int i = 0; i < threadCount; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        var localInput = CloneInputForConcurrency(input);
                        barrier.SignalAndWait();
                        algorithm.Execute(localInput);
                    });
                }
                
                barrier.SignalAndWait(); // Launch!
                sw.Start();
                Task.WaitAll(tasks);
                sw.Stop();
            }

            return sw.Elapsed.TotalMilliseconds;
        }

        private object CloneInputForConcurrency(object input)
        {
            // Deterministic dataset sharing requires cloning array elements explicitly
            if (input is int[] iArr) return (int[])iArr.Clone();
            if (input is float[] fArr) return (float[])fArr.Clone();
            if (input is string[] sArr) return (string[])sArr.Clone();
            return input;
        }
    }
}
