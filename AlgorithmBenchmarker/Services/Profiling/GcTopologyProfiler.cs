using System;
using System.Diagnostics;
using AlgorithmBenchmarker.Algorithms;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class GcTopologyResult
    {
        public long TotalAllocatedBytes { get; set; }
        public int Gen0Collections { get; set; }
        public int Gen1Collections { get; set; }
        public int Gen2Collections { get; set; }
        public long CohAllocatedBytes { get; set; } // Represented via approximate fragment difference or large object.
        public double AllocationPressureRateBytesPerMs { get; set; }
        public double GcPressureScore { get; set; } // Normalization scalar
    }

    /// <summary>
    /// Managed Runtime GC Topology Profiler.
    /// Tracks allocation frequencies, lifespans, and generation collections.
    /// </summary>
    public static class GcTopologyProfiler
    {
        public static GcTopologyResult ProfileExecution(IAlgorithm algorithm, object input)
        {
            // Force clean state
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();

            long initialBytes = GC.GetAllocatedBytesForCurrentThread();
            int gen0Init = GC.CollectionCount(0);
            int gen1Init = GC.CollectionCount(1);
            int gen2Init = GC.CollectionCount(2);

            var sw = Stopwatch.StartNew();
            algorithm.Execute(input);
            sw.Stop();

            long finalBytes = GC.GetAllocatedBytesForCurrentThread();
            int gen0Final = GC.CollectionCount(0);
            int gen1Final = GC.CollectionCount(1);
            int gen2Final = GC.CollectionCount(2);

            long totalAllocated = finalBytes - initialBytes;
            int g0 = gen0Final - gen0Init;
            int g1 = gen1Final - gen1Init;
            int g2 = gen2Final - gen2Init;
            
            double elapsedMs = sw.Elapsed.TotalMilliseconds;
            double rate = elapsedMs > 0 ? totalAllocated / elapsedMs : 0;

            // Heuristic GC pressure score (Weighting: Gen0=1, Gen1=5, Gen2=20, plus standard pressure)
            double gcScore = (g0 * 1.0) + (g1 * 5.0) + (g2 * 20.0) + (totalAllocated / 1024.0 / 1024.0);

            return new GcTopologyResult
            {
                TotalAllocatedBytes = totalAllocated,
                Gen0Collections = g0,
                Gen1Collections = g1,
                Gen2Collections = g2,
                AllocationPressureRateBytesPerMs = rate,
                GcPressureScore = gcScore
            };
        }
    }
}
