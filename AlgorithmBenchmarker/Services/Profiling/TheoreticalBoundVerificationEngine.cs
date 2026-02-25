using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class RegressionFitResult
    {
        public string ModelName { get; set; } = string.Empty;
        public double BestFitConstant { get; set; }
        public double RSquared { get; set; }
        public double[] Residuals { get; set; } = Array.Empty<double>();
    }

    /// <summary>
    /// Theoretical Bound Verification Engine.
    /// Performs numerically stable linear regression of empirical T(N) against theoretical complexity bounds.
    /// </summary>
    public class TheoreticalBoundVerificationEngine
    {
        public List<RegressionFitResult> VerifyComplexityBounds(int[] nValues, double[] tValues)
        {
            if (nValues.Length != tValues.Length || nValues.Length < 3)
                throw new ArgumentException("Requires symmetric N and T arrays with minimum length 3.");

            var results = new List<RegressionFitResult>();

            // Linear O(N)
            results.Add(FitModel(nValues, tValues, x => x, "O(N)"));

            // Log-Linear O(N log N)
            results.Add(FitModel(nValues, tValues, x => x * Math.Log(x, 2), "O(N log N)"));

            // Quadratic O(N^2)
            results.Add(FitModel(nValues, tValues, x => (double)x * x, "O(N^2)"));

            // Cubic O(N^3)
            results.Add(FitModel(nValues, tValues, x => (double)x * x * x, "O(N^3)"));

            // Sort by Best Fit (Highest R^2)
            results = results.OrderByDescending(r => r.RSquared).ToList();
            return results;
        }

        private RegressionFitResult FitModel(int[] nValues, double[] tValues, Func<int, double> modelTransformation, string modelName)
        {
            var fValues = nValues.Select(modelTransformation).ToArray();
            
            // Linear regression: T = c * F(N) + error
            // Best fit constant c = Sum(F(N_i) * T_i) / Sum(F(N_i)^2)
            
            double sumOfProducts = 0;
            double sumOfSquares = 0;

            for (int i = 0; i < nValues.Length; i++)
            {
                sumOfProducts += fValues[i] * tValues[i];
                sumOfSquares += fValues[i] * fValues[i];
            }

            double c = 0;
            if (sumOfSquares > 0)
                c = sumOfProducts / sumOfSquares;

            // Calculate R^2 (Coefficient of Determination)
            double meanT = tValues.Average();
            double ssTot = 0;
            double ssRes = 0;
            double[] residuals = new double[nValues.Length];

            for (int i = 0; i < nValues.Length; i++)
            {
                double predicted = c * fValues[i];
                double actual = tValues[i];
                double d = actual - meanT;
                ssTot += d * d;

                double residual = actual - predicted;
                residuals[i] = residual;
                ssRes += residual * residual;
            }

            double rSquared = 1.0;
            if (ssTot > 0)
            {
                rSquared = 1.0 - (ssRes / ssTot);
            }
            if (rSquared < 0) rSquared = 0; // Negative R^2 implies model is arbitrarily worse than flat mean.

            return new RegressionFitResult
            {
                ModelName = modelName,
                BestFitConstant = c,
                RSquared = rSquared,
                Residuals = residuals
            };
        }
    }
}
