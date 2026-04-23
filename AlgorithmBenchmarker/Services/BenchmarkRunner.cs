using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Services.Instrumentation;
using AlgorithmBenchmarker.Services.Profiling;

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

                // Create a config specific for this iteration using Clone()
                var iterationConfig = config.Clone();
                iterationConfig.InputSize = size;

                var result = RunSingle(algorithm, iterationConfig, batchId, token);
                if (result != null) results.Add(result);

                currentStep++;
                progress?.Report((int)((double)currentStep / totalSteps * 100));
            }

            // 9. Theoretical Bounds Verification Hook (Post-Batch)
            if (config.EnableTheoreticalBoundsVerification && results.Count >= 3)
            {
                var nValues = results.Select(r => r.InputSize).ToArray();
                var tValues = results.Select(r => r.AvgTimeMs).ToArray();
                var verifier = new TheoreticalBoundVerificationEngine();
                var fits = verifier.VerifyComplexityBounds(nValues, tValues);
                var bestFit = fits.First();

                foreach (var result in results)
                {
                    result.ExtendedMetrics["Bounds_BestFitModel"] = bestFit.ModelName;
                    result.ExtendedMetrics["Bounds_RSquared"] = bestFit.RSquared.ToString("F4");
                    result.ExtendedMetrics["Bounds_Constant"] = bestFit.BestFitConstant.ToString("E4");
                }
            }

            return results;
        }

        public PhaseTransitionResult RunPhaseTransitionSweep(IAlgorithm algorithm, BenchmarkConfig config)
        {
            if (!config.EnablePhaseTransitionDetector) return new PhaseTransitionResult();
            var detector = new AlgorithmicPhaseTransitionDetector(_inputGenerator);
            return detector.DetectPhaseTransition(algorithm, config, config.PhaseTransitionSweepStart, config.PhaseTransitionSweepEnd, config.PhaseTransitionSteps, config.PhaseTransitionSweepParameter);
        }

        public List<DragRaceResult> RunDragRace(List<IAlgorithm> algorithms, BenchmarkConfig config, CancellationToken token)
        {
            if (!config.EnableDragRaceMode || algorithms.Count < 2) return new List<DragRaceResult>();

            // Generate shared dataset
            var masterInput = _inputGenerator.GenerateInput(config, algorithms[0].Category, "DragRace");
            
            var orchestrator = new DragRaceOrchestrator();
            return orchestrator.RunDragRace(algorithms, masterInput);
        }

        private BenchmarkResult? RunSingle(IAlgorithm algorithm, BenchmarkConfig config, string batchId, CancellationToken token)
        {
            var masterInput = _inputGenerator.GenerateInput(config, algorithm.Category, algorithm.Name);

            // Research-Grade Extensions Initialization
            var extendedMetrics = new Dictionary<string, string>();

            // 1. Thread Scaling (Amdahl) Hook
            if (config.EnableThreadScalingAnalysis)
            {
                var amdahl = new AmdahlAnalyzer();
                var threadScaling = amdahl.AnalyzeScaling(algorithm, masterInput, config.MaxThreadsForScaling);
                extendedMetrics["Amdahl_Efficiency_Max"] = threadScaling.Last().Efficiency.ToString("F4");
                extendedMetrics["Amdahl_Speedup_Max"] = threadScaling.Last().Speedup.ToString("F4");
                extendedMetrics["Amdahl_SerialFraction"] = threadScaling.Last().EstimatedSerialFraction.ToString("F4");
            }

            // 2. JIT Warmup Profiler Hook
            if (config.EnableJitWarmupProfiler)
            {
                var jitProfiler = new JitWarmupProfiler();
                var jitResults = jitProfiler.ProfileJitWarmup(algorithm, masterInput, config.JitIterations, config.JitForceGc);
                var inflection = jitResults.FirstOrDefault(r => r.IsWarmupInflectionPoint);
                if (inflection != null)
                {
                    extendedMetrics["JIT_Inflection_Iteration"] = inflection.Iteration.ToString();
                    extendedMetrics["JIT_Inflection_TimeMs"] = inflection.ExecutionTimeMs.ToString("F4");
                }
            }

            // 11. GC Topology Profiler Hook
            if (config.EnableGcTopologyProfiler)
            {
                var gcResult = GcTopologyProfiler.ProfileExecution(algorithm, masterInput);
                extendedMetrics["GC_TotalAllocated"] = gcResult.TotalAllocatedBytes.ToString();
                extendedMetrics["GC_Gen0"] = gcResult.Gen0Collections.ToString();
                extendedMetrics["GC_Gen1"] = gcResult.Gen1Collections.ToString();
                extendedMetrics["GC_Gen2"] = gcResult.Gen2Collections.ToString();
                extendedMetrics["GC_PressureScore"] = gcResult.GcPressureScore.ToString("F4");
            }

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

                // 3. Micro-Operation Tracer Hook
                if (config.EnableMicroTracer) ExecutionTracer.StartTracing();
                // 8. Cache Locality Hook
                if (config.EnableCacheLocalityAnalyzer && i == 0) { CacheLocalityAnalyzer.CacheLineSizeBytes = config.CacheLineSizeBytes; CacheLocalityAnalyzer.StartTracking(); }

                algorithm.Execute(input);

                if (config.EnableCacheLocalityAnalyzer && i == 0)
                {
                    var cacheResult = CacheLocalityAnalyzer.StopAndAnalyze();
                    extendedMetrics["Cache_LocalityScore"] = cacheResult.LocalityScore.ToString("F4");
                    extendedMetrics["Cache_MissProbability"] = cacheResult.EstimatedCacheMissProbability.ToString("F4");
                }
                if (config.EnableMicroTracer)
                {
                    ExecutionTracer.StopTracing();
                    if (i == 0) // Only record trace metrics of the first rep to avoid duplicating identical deterministic outputs
                    {
                        var trace = ExecutionTracer.GetTrace();
                        extendedMetrics["MicroTracer_TotalOps"] = trace.Count.ToString();
                        extendedMetrics["MicroTracer_Comparisons"] = trace.Count(x => x.Type == OperationType.Comparison).ToString();
                        extendedMetrics["MicroTracer_Swaps"] = trace.Count(x => x.Type == OperationType.Swap).ToString();
                    }
                }

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

            long avgAllocated = config.Repetitions > 0 ? totalAllocated / config.Repetitions : 0;

            // 7. Energy Estimator Hook
            if (config.EnableEnergyEstimator)
            {
                var estimator = new EnergyEstimator();
                long totalOps = extendedMetrics.ContainsKey("MicroTracer_TotalOps") ? long.Parse(extendedMetrics["MicroTracer_TotalOps"]) : 0;
                var energy = estimator.EstimateFromTimeAndMemory(avgTime, avgAllocated, (int)totalOps);
                extendedMetrics["Energy_ModeledJoules"] = energy.EstimatedJoules.ToString("E4");
                extendedMetrics["Energy_ModeledCarbonGrams"] = energy.EstimatedCarbonGrams.ToString("E4");
            }

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
                AllocatedBytes = avgAllocated,
                MemoryBytes = avgAllocated, // Mapping to old field too just in case
                ExtendedMetrics = extendedMetrics,
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
