namespace AlgorithmBenchmarker.Algorithms.Sorting
{
    public class HeapSort : IAlgorithm
    {
         public string Name => "Heap Sort";
         public string Category => "Sorting";
         public string Complexity => "O(N log N)";
        public override string ToString() => Name;
        public void Execute(object input)
         {
             if (input is int[] arr)
             {
                 int n = arr.Length;
                 for (int i = n / 2 - 1; i >= 0; i--)
                     Heapify(arr, n, i);
                 for (int i = n - 1; i > 0; i--)
                 {
                     (arr[0], arr[i]) = (arr[i], arr[0]);
                     Heapify(arr, i, 0);
                 }
             }
         }

         private void Heapify(int[] arr, int n, int i)
         {
             int largest = i;
             int l = 2 * i + 1;
             int r = 2 * i + 2;

             if (l < n && arr[l] > arr[largest]) largest = l;
             if (r < n && arr[r] > arr[largest]) largest = r;

             if (largest != i)
             {
                 (arr[i], arr[largest]) = (arr[largest], arr[i]);
                 Heapify(arr, n, largest);
             }
         }
    }
}
