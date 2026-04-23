using System;
using System.Collections.Generic;
using System.Diagnostics;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Services;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class PhaseTransitionResult
    {
        public double CriticalParameter { get; set; }
        public double MaxSlopeDelta { get; set; }
        public Dictionary<double, double> ParameterToLatency { get; set; } = new Dictionary<double, double>();
    }

    /// <summary>
    /// Algorithmic Phase Transition Detector.
    /// Maps continuous numeric constraint sweeps (e.g. Graph Density 0.0 to 1.0) and identifies algorithmic phase transitions (discontinuities in gradient T(p)).
    /// </summary>
    public class AlgorithmicPhaseTransitionDetector
    {
        private readonly InputGenerator _generator;

        public AlgorithmicPhaseTransitionDetector(InputGenerator generator)
        {
            _generator = generator;
        }

        public PhaseTransitionResult DetectPhaseTransition(IAlgorithm algorithm, BenchmarkConfig baseConfig, double sweepStart, double sweepEnd, int steps, string sweepParameter = "Density")
        {
            var parameterToLatency = new Dictionary<double, double>();
            double stepSize = (sweepEnd - sweepStart) / steps;
            
            // Execute Sweep
            for (double p = sweepStart; p <= sweepEnd; p += stepSize)
            {
                // Isolate memory
                GC.Collect();
                GC.WaitForPendingFinalizers();

                var configCopy = CloneConfig(baseConfig);
                
                // Inject numeric string proxy - existing architecture maps density string internally.
                if (sweepParameter == "Density") configCopy.GraphDensity = (p > 0.5 ? "Dense" : (p < 0.2 ? "Sparse" : "Medium")); 

                // Or map standard integer param proxy
                if (sweepParameter == "KeySize") configCopy.KeySize = (int)p;

                var input = _generator.GenerateInput(configCopy, algorithm.Category, algorithm.Name);

                var sw = Stopwatch.StartNew();
                algorithm.Execute(input);
                sw.Stop();

                parameterToLatency[p] = sw.Elapsed.TotalMilliseconds;
            }

            // Detect Second Derivative Maximum (Slope Discontinuity)
            double criticalP = sweepStart;
            double maxSlopeDelta = 0;
            
            var pList = new List<double>(parameterToLatency.Keys);
            pList.Sort();

            for (int i = 1; i < pList.Count - 1; i++)
            {
                double prevSlope = (parameterToLatency[pList[i]] - parameterToLatency[pList[i-1]]) / stepSize;
                double nextSlope = (parameterToLatency[pList[i+1]] - parameterToLatency[pList[i]]) / stepSize;
                
                double slopeDelta = Math.Abs(nextSlope - prevSlope);
                if (slopeDelta > maxSlopeDelta)
                {
                    maxSlopeDelta = slopeDelta;
                    criticalP = pList[i];
                }
            }

            return new PhaseTransitionResult
            {
                ParameterToLatency = parameterToLatency,
                MaxSlopeDelta = maxSlopeDelta,
                CriticalParameter = criticalP
            };
        }

        private BenchmarkConfig CloneConfig(BenchmarkConfig source)
        {
            return source.Clone();
        }
    }
}
