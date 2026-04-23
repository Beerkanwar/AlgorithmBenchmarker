using System;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.Services.Adversarial
{
    /// <summary>
    /// Pluggable adversarial strategy interface.
    /// Provides worst-case inputs for specific algorithms.
    /// </summary>
    public interface IAdversarialGenerator
    {
        string TargetAlgorithm { get; }
        string TriggerExplanation { get; }
        object GeneratePathologicalInput(int size, int seed);
    }
}
