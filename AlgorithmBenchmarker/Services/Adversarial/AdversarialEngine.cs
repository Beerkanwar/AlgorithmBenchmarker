using System.Collections.Generic;
using System.Linq;

namespace AlgorithmBenchmarker.Services.Adversarial
{
    public class AdversarialEngine
    {
        private readonly Dictionary<string, IAdversarialGenerator> _generators;

        public AdversarialEngine()
        {
            _generators = new Dictionary<string, IAdversarialGenerator>();
            Register(new AdversarialQuickSortGenerator());
            Register(new AdversarialHashGenerator());
            Register(new AdversarialBSTGenerator());
        }

        public void Register(IAdversarialGenerator generator)
        {
            _generators[generator.TargetAlgorithm] = generator;
        }

        public bool HasAdversarialGenerator(string targetAlgorithm)
        {
            return _generators.ContainsKey(targetAlgorithm);
        }

        public object? Generate(string targetAlgorithm, int size, int seed)
        {
            if (_generators.TryGetValue(targetAlgorithm, out var gen))
            {
                return gen.GeneratePathologicalInput(size, seed);
            }
            return null;
        }
    }
}
