namespace AlgorithmBenchmarker.Algorithms.Searching
{
    public class LinearSearch : IAlgorithm
    {
        public string Name => "Linear Search";
        public string Category => "Searching";
        public string Complexity => "O(N)";

        public void Execute(object input)
        {
            if (input is int[] arr && arr.Length > 0)
            {
                int target = arr[arr.Length - 1]; 
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == target) return;
                }
            }
        }
    }
}
