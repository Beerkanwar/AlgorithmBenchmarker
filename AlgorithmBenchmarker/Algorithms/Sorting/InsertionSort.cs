namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class InsertionSort : IAlgorithm
    {
        public string Name => "Insertion Sort";
        public string Category => "Sorting";
        public string Complexity => "O(N^2)";
        public override string ToString() => Name;
        public void Execute(object input)
        {
            if (input is int[] arr)
            {
                int n = arr.Length;
                for (int i = 1; i < n; ++i)
                {
                    int key = arr[i];
                    int j = i - 1;
                    while (j >= 0 && arr[j] > key)
                    {
                        arr[j + 1] = arr[j];
                        j = j - 1;
                    }
                    arr[j + 1] = key;
                }
            }
        }
    }
}
