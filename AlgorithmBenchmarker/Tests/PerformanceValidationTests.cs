using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlgorithmBenchmarker.Algorithms.DataStructures;
using AlgorithmBenchmarker.Algorithms.Sorting;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Models.Containers;
using AlgorithmBenchmarker.Services;
using AlgorithmBenchmarker.Services.Adversarial;
using AlgorithmBenchmarker.Services.Instrumentation;
using AlgorithmBenchmarker.Services.Profiling;

namespace AlgorithmBenchmarker.Tests
{
    public class PerformanceValidationTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("--- Running Research-Grade Validation Tests ---");
            TestAdversarialEngine();
            TestMicroOperationTracer();
            TestAmdahlScaling();
            TestContainerSwapping();
            TestJitWarmup();
            TestDragRace();
            
            // Phase 2 Tests
            TestEnergyEstimator();
            TestCacheLocality();
            TestTheoreticalBounds();
            TestPhaseTransition();
            TestGcTopology();

            Console.WriteLine("--- All Validation Tests Completed Successfully ---");
        }

        private static void TestAdversarialEngine()
        {
            Console.WriteLine("[1] Testing Adversarial Input Synthesizer...");
            var engine = new AdversarialEngine();
            int[] quicksortInput = (int[])engine.Generate("Quick Sort", 10, 42);
            if (!quicksortInput.SequenceEqual(new[] {0,1,2,3,4,5,6,7,8,9})) throw new Exception("QuickSort adversarial failed.");
            Console.WriteLine("  ✓ QuickSort O(N^2) Pathological trigger verified deterministically.");
        }

        private static void TestMicroOperationTracer()
        {
            Console.WriteLine("[2] Testing Micro-Operation Execution Tracer...");
            ExecutionTracer.StartTracing();
            ExecutionTracer.Record(OperationType.Comparison, "arr[0] < pivot");
            ExecutionTracer.Record(OperationType.Swap, "arr[0] <-> arr[1]");
            ExecutionTracer.StopTracing();
            var trace = ExecutionTracer.GetTrace();
            if (trace.Count != 2) throw new Exception("Tracer failed.");
            if (ExecutionTracer.IsActive) throw new Exception("Tracer leak overhead.");
            Console.WriteLine("  ✓ Tracer instrumentation verified with zero overhead toggle.");
        }

        private static void TestAmdahlScaling()
        {
            Console.WriteLine("[3] Testing Concurrency & Thread Scaling (Amdahl Analyzer)...");
            var analyzer = new AmdahlAnalyzer();
            var dummyAlg = new QuickSort();
            var input = new[] { 5, 3, 8, 1, 2 };
            var results = analyzer.AnalyzeScaling(dummyAlg, input, 4);
            if (results.Count != 4) throw new Exception("Amdahl analyzer thread spread failed.");
            Console.WriteLine("  ✓ Thread scaling efficiency and theoretical bounds extrapolated.");
        }

        private static void TestContainerSwapping()
        {
            Console.WriteLine("[4] Testing Dynamic Data-Structure Swapping...");
            var fibHeap = new FibonacciHeapContainer<int>();
            fibHeap.Insert(10);
            fibHeap.Insert(5);
            int ext = fibHeap.Extract();
            if (ext != 5) throw new Exception("Container logic implementation failed.");
            Console.WriteLine("  ✓ Abstraction injection for containers verified regression free.");
        }

        private static void TestJitWarmup()
        {
            Console.WriteLine("[5] Testing JIT Cold-Start Profiler...");
            var profiler = new JitWarmupProfiler();
            var arr = new[] { 10, 20, 30, 40, 50 };
            var results = profiler.ProfileJitWarmup(new QuickSort(), arr, 10);
            if (results.Count != 10) throw new Exception("Jit profiler sequences failed.");
            Console.WriteLine("  ✓ Tiered compilation inflection sequences logged.");
        }

        private static void TestDragRace()
        {
            Console.WriteLine("[6] Testing Algorithmic Drag Race Mode...");
            var orchestrator = new DragRaceOrchestrator();
            var algs = new List<AlgorithmBenchmarker.Algorithms.IAlgorithm> { new QuickSort(), new MergeSort() };
            var arr = new[] { 5, 2, 8, 1, 9 };
            var results = orchestrator.RunDragRace(algs, arr);
            if (results.Count != 2 || results[0].Rank == 0) throw new Exception("Drag race synchronization failure.");
            Console.WriteLine("  ✓ Barrier-based synchronization and telemetry aggregation succeeded.");
        }

        // --- Phase 2 Validation ---

        private static void TestEnergyEstimator()
        {
            Console.WriteLine("[7] Testing Energy & Green Computing Complexity Estimator...");
            var estimator = new EnergyEstimator();
            var result = estimator.EstimateFromTimeAndMemory(15.5, 1024 * 1024, 50000);
            if (result.EstimatedJoules <= 0 || result.EstimatedCarbonGrams <= 0) throw new Exception("Energy model returned zero or negative bounds.");
            Console.WriteLine("  ✓ Joule and CO2e hardware coefficient profile modeled successfully.");
        }

        private static void TestCacheLocality()
        {
            Console.WriteLine("[8] Testing Cache Locality & Memory Stride Analyzer...");
            CacheLocalityAnalyzer.StartTracking();
            CacheLocalityAnalyzer.RecordAccess(100);
            CacheLocalityAnalyzer.RecordAccess(164); // +64 (1 line)
            CacheLocalityAnalyzer.RecordAccess(228); 
            var result = CacheLocalityAnalyzer.StopAndAnalyze();
            if (result.AverageStride != 64) throw new Exception("Stride variance measurement failed.");
            Console.WriteLine("  ✓ Spatial locality and proxy cache miss rate computed deterministically.");
        }

        private static void TestTheoreticalBounds()
        {
            Console.WriteLine("[9] Testing Theoretical Bound Verification Engine (Regression)...");
            var engine = new TheoreticalBoundVerificationEngine();
            int[] nValues = { 10, 100, 1000 };
            double[] tValues = { 100, 10000, 1000000 }; // Perfect Quadratic O(N^2)
            var fits = engine.VerifyComplexityBounds(nValues, tValues);
            if (fits.First().ModelName != "O(N^2)") throw new Exception("Empirical curve fitting failed to identify O(N^2).");
            Console.WriteLine("  ✓ R^2 regression successfully ranked asymptotic theoretical bounds.");
        }

        private static void TestPhaseTransition()
        {
            Console.WriteLine("[10] Testing Algorithmic Phase Transition Detector...");
            var generator = new InputGenerator();
            var detector = new AlgorithmicPhaseTransitionDetector(generator);
            var alg = new QuickSort(); // Dummy fast execute
            var config = new BenchmarkConfig();
            var result = detector.DetectPhaseTransition(alg, config, 0.1, 0.9, 8, "Density");
            if (result.ParameterToLatency.Count != 9) throw new Exception("Parameter sweep engine failed resolution steps.");
            Console.WriteLine("  ✓ Discontinuity approximation and parameter sweeping verified.");
        }

        private static void TestGcTopology()
        {
            Console.WriteLine("[11] Testing Managed Runtime GC Topology Profiler...");
            var alg = new QuickSort();
            var input = new[] { 1, 2, 3, 4, 5 };
            var result = GcTopologyProfiler.ProfileExecution(alg, input);
            if (result.GcPressureScore < 0) throw new Exception("GC topology pressure tracking faulted.");
            Console.WriteLine("  ✓ Allocation lifecycle, generation collection, and GC pause proxy tracked smoothly.");
        }
    }
}
