using System;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class CacheLocalityResult
    {
        public double AverageStride { get; set; }
        public double StrideVariance { get; set; }
        public double CacheLineCrossingRate { get; set; }
        public double EstimatedCacheMissProbability { get; set; }
        public double LocalityScore { get; set; } // 0.0 to 1.0
    }

    /// <summary>
    /// Cache Locality & Memory Stride Analyzer.
    /// Instruments simulated memory access patterns strictly distinct from algorithmic logic.
    /// </summary>
    public static class CacheLocalityAnalyzer
    {
        [ThreadStatic]
        private static bool _isActive;
        [ThreadStatic]
        private static List<long>? _accessStream;

        public static int CacheLineSizeBytes { get; set; } = 64;

        public static void StartTracking()
        {
            _isActive = true;
            _accessStream = new List<long>(10000);
        }

        public static void RecordAccess(long virtualAddressIdentifier)
        {
            if (!_isActive || _accessStream == null) return;
            _accessStream.Add(virtualAddressIdentifier);
        }

        public static CacheLocalityResult StopAndAnalyze()
        {
            _isActive = false;
            if (_accessStream == null || _accessStream.Count < 2)
                return new CacheLocalityResult { LocalityScore = 1.0 }; // Trivial 

            long totalStride = 0;
            long maxStride = 0;
            int crossings = 0;

            var strides = new List<long>(_accessStream.Count);

            for (int i = 1; i < _accessStream.Count; i++)
            {
                long prev = _accessStream[i - 1];
                long curr = _accessStream[i];
                long stride = Math.Abs(curr - prev);
                
                strides.Add(stride);
                totalStride += stride;
                if (stride > maxStride) maxStride = stride;

                // Typical Cache line crossing:
                if ((prev / CacheLineSizeBytes) != (curr / CacheLineSizeBytes))
                {
                    crossings++;
                }
            }

            double avgStride = (double)totalStride / strides.Count;

            double sumSq = 0;
            foreach (var s in strides)
            {
                sumSq += (s - avgStride) * (s - avgStride);
            }
            double variance = sumSq / strides.Count;

            double crossingRate = (double)crossings / strides.Count;

            // Simplified Miss Probability. If crossing rate is high and stride > line size, high probability.
            double missProb = Math.Min(1.0, crossingRate * (avgStride / CacheLineSizeBytes));
            double score = Math.Max(0.0, 1.0 - missProb);

            return new CacheLocalityResult
            {
                AverageStride = avgStride,
                StrideVariance = variance,
                CacheLineCrossingRate = crossingRate,
                EstimatedCacheMissProbability = missProb,
                LocalityScore = score
            };
        }
    }
}
