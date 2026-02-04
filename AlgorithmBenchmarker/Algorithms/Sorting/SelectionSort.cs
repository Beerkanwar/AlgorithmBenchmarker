namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class SelectionSort : IAlgorithm
    {
        public string Name => "Selection Sort";
        public string Category => "Sorting";
        public string Complexity => "O(N^2)";

        public void Execute(object input)
        {
            if (input is int[] arr)
            {
                int n = arr.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    int min_idx = i;
                    for (int j = i + 1; j < n; j++)
                        if (arr[j] < arr[min_idx])
                            min_idx = j;

                    (arr[min_idx], arr[i]) = (arr[i], arr[min_idx]);
                }
            }
        }
    }
}
