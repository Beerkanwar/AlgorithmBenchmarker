using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AlgorithmBenchmarker.Algorithms;

namespace AlgorithmBenchmarker.Services.Profiling
{
    public class DragRaceResult
    {
        public string AlgorithmName { get; set; } = string.Empty;
        public double ExecutionTimeMs { get; set; }
        public long AllocatedBytes { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// Algorithmic Drag Race Mode.
    /// Synchronized multi-algorithm benchmarking barrier.
    /// </summary>
    public class DragRaceOrchestrator
    {
        public List<DragRaceResult> RunDragRace(List<IAlgorithm> competitors, object baseInput)
        {
            int n = competitors.Count;
            if (n < 2) return new List<DragRaceResult>();

            var results = new DragRaceResult[n];
            var tasks = new Task[n];
            using (var barrier = new Barrier(n + 1))
            {
                for (int i = 0; i < n; i++)
                {
                    int index = i;
                    var alg = competitors[index];
                    var inputCopy = CloneInput(baseInput);

                    tasks[index] = Task.Run(() =>
                    {
                        var sw = new Stopwatch();
                        barrier.SignalAndWait(); // Wait for all competitors and main thread
                        
                        long startBytes = System.GC.GetAllocatedBytesForCurrentThread();
                        sw.Start();
                        alg.Execute(inputCopy);
                        sw.Stop();
                        long endBytes = System.GC.GetAllocatedBytesForCurrentThread();

                        results[index] = new DragRaceResult
                        {
                            AlgorithmName = alg.Name,
                            ExecutionTimeMs = sw.Elapsed.TotalMilliseconds,
                            AllocatedBytes = endBytes - startBytes
                        };
                    });
                }
                
                barrier.SignalAndWait(); // Launch!
                Task.WaitAll(tasks);
            }

            var finalResults = new List<DragRaceResult>(results);
            finalResults.Sort((a, b) => a.ExecutionTimeMs.CompareTo(b.ExecutionTimeMs));
            
            for (int i = 0; i < finalResults.Count; i++)
            {
                finalResults[i].Rank = i + 1;
            }

            return finalResults;
        }

        private object CloneInput(object input)
        {
            if (input is int[] iArr) return (int[])iArr.Clone();
            if (input is float[] fArr) return (float[])fArr.Clone();
            if (input is string[] sArr) return (string[])sArr.Clone();
            return input;
        }
    }
}
