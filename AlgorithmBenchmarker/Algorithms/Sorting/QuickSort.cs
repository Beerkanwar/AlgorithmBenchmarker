namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class QuickSort : IAlgorithm
    {
        public string Name => "Quick Sort";
        public string Category => "Sorting";
        public string Complexity => "O(N log N)";

        public override string ToString() => Name;

        public void Execute(object input)
        {
            if (input is int[] array)
            {
                Sort(array, 0, array.Length - 1);
            }
        }

        private void Sort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(arr, low, high);
                Sort(arr, low, pi - 1);
                Sort(arr, pi + 1, high);
            }
        }

        private int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = (low - 1);
            for (int j = low; j < high; j++)
            {
                if (arr[j] < pivot)
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
            }
            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }
    }
}
