namespace AlgorithmBenchmarker.Algorithms.Searching
{
    public class InterpolationSearch : IAlgorithm
    {
        public string Name => "Interpolation Search";
        public string Category => "Searching";
        public string Complexity => "O(log(log N))";

        public void Execute(object input)
        {
             if (input is int[] arr && arr.Length > 0)
            {
                int lo = 0, hi = (arr.Length - 1);
                int x = arr[hi]; 

                while (lo <= hi && x >= arr[lo] && x <= arr[hi])
                {
                    if (lo == hi)
                    {
                        if (arr[lo] == x) return;
                        return;
                    }
                    
                    int pos = lo + (int)(((long)(hi - lo) * (x - arr[lo])) / (arr[hi] - arr[lo]));
                    
                    if (pos < 0 || pos >= arr.Length) break; 

                    if (arr[pos] == x) return;
                    if (arr[pos] < x) lo = pos + 1;
                    else hi = pos - 1;
                }
            }
        }
    }
}
