using System.Threading;
using AlgorithmBenchmarker.Models;

namespace AlgorithmBenchmarker.Models
{
    public static class BenchmarkContext
    {
        private static AsyncLocal<BenchmarkConfig> _currentConfig = new AsyncLocal<BenchmarkConfig>();

        public static BenchmarkConfig Current
        {
            get => _currentConfig.Value;
            set => _currentConfig.Value = value;
        }
    }
}
