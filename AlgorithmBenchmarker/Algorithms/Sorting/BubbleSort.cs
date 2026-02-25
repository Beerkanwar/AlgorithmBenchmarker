namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class BubbleSort : IAlgorithm
    {
        public string Name => "Bubble Sort";
        public string Category => "Sorting";
        public string Complexity => "O(N^2)";

        public override string ToString() => Name;  // Add this line

        public void Execute(object input)
        {
            if (input is int[] array)
            {
                int n = array.Length;
                for (int i = 0; i < n - 1; i++)
                    for (int j = 0; j < n - i - 1; j++)
                        if (array[j] > array[j + 1])
                            (array[j], array[j + 1]) = (array[j + 1], array[j]);
            }
        }
    }
}
