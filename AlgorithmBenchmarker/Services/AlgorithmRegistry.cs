using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlgorithmBenchmarker.Algorithms;

namespace AlgorithmBenchmarker.Services
{
    public class AlgorithmRegistry
    {
        public List<IAlgorithm> Algorithms { get; private set; } = new List<IAlgorithm>();

        public AlgorithmRegistry()
        {
            DiscoverAlgorithms();
        }

        private void DiscoverAlgorithms()
        {
            var interfaceType = typeof(IAlgorithm);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is IAlgorithm algorithm)
                {
                    Algorithms.Add(algorithm);
                }
            }
        }

        public IEnumerable<string> GetCategories()
        {
            return Algorithms.Select(a => a.Category).Distinct();
        }

        public IEnumerable<IAlgorithm> GetAlgorithmsByCategory(string category)
        {
            return Algorithms.Where(a => a.Category == category);
        }
    }
}
