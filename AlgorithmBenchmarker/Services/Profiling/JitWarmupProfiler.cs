using System;
using System.Collections.Generic;
using System.Diagnostics;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class JitIterationResult
    {
        public int Iteration { get; set; }
        public double ExecutionTimeMs { get; set; }
        public bool IsWarmupInflectionPoint { get; set; }
    }

    /// <summary>
    /// JIT Cold-Start & Tiered Compilation Profiler (.NET 8).
    /// Detects compilation transition by analyzing sequential execution time drops.
    /// </summary>
    public class JitWarmupProfiler
    {
        public List<JitIterationResult> ProfileJitWarmup(IAlgorithm algorithm, object baseInput, int totalIterations = 100, bool forceGc = false)
        {
            var results = new List<JitIterationResult>();
            double previousTime = double.MaxValue;
            int inflectionPoint = -1;

            for (int i = 1; i <= totalIterations; i++)
            {
                if (forceGc)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                object input = CloneInput(baseInput);
                var sw = Stopwatch.StartNew();
                algorithm.Execute(input);
                sw.Stop();

                double elapsedMs = sw.Elapsed.TotalMilliseconds;
                
                // Warmup heuristic: Detect when execution time stabilizes (drops by more than 50% relative to average of first few runs)
                // or just absolute drop from iter 1.
                if (i > 1 && inflectionPoint == -1 && elapsedMs < previousTime * 0.6)
                {
                    inflectionPoint = i;
                }

                results.Add(new JitIterationResult
                {
                    Iteration = i,
                    ExecutionTimeMs = elapsedMs,
                    IsWarmupInflectionPoint = (i == inflectionPoint)
                });

                previousTime = elapsedMs;
            }

            return results;
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
