namespace AlgorithmBenchmarker.Models
{
    public class MLInputData
    {
        // Features Matrix: [Samples, Features]
        public double[][] Features { get; set; }
        
        // Labels: [Samples] (0 or 1 for Classification, Value for Regression)
        public double[] Labels { get; set; }

        public MLInputData(double[][] features, double[] labels)
        {
            Features = features;
            Labels = labels;
        }
    }
}
