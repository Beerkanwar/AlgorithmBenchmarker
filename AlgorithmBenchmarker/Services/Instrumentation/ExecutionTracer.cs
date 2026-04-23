using System;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.Services.Instrumentation
{
    /// <summary>
    /// Deterministic instruction-level instrumentation layer.
    /// Uses ThreadStatic to maintain zero instrumentation overhead when toggled off.
    /// </summary>
    public static class ExecutionTracer
    {
        [ThreadStatic]
        private static bool _isActive;
        
        [ThreadStatic]
        private static List<OperationRecord>? _operations;

        public static bool IsActive => _isActive;

        public static void StartTracing()
        {
            _isActive = true;
            _operations = new List<OperationRecord>();
        }

        public static void StopTracing()
        {
            _isActive = false;
        }

        public static List<OperationRecord> GetTrace()
        {
            return _operations ?? new List<OperationRecord>();
        }

        public static void Record(OperationType type, string details = "")
        {
            if (!_isActive || _operations == null) return;
            _operations.Add(new OperationRecord(type, details));
        }

        // Frame-by-frame stepping API (yields records deterministically)
        public static IEnumerable<OperationRecord> StepReplay()
        {
            if (_operations == null) yield break;
            foreach (var op in _operations)
            {
                yield return op;
            }
        }
    }
}
