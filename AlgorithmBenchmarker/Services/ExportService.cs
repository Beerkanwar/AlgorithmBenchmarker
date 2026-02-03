using System.Collections.Generic;
using System.IO;
using System.Text;
using AlgorithmBenchmarker.Models;
using System.Text.Json;

namespace AlgorithmBenchmarker.Services
{
    public class ExportService
    {
        public void ExportToCsv(IEnumerable<BenchmarkResult> results, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Algorithm,Category,Input Size,Time (ms),Memory (Bytes),Timestamp");

            foreach (var r in results)
            {
                sb.AppendLine($"{r.AlgorithmName},{r.Category},{r.InputSize},{r.AvgTimeMs},{r.MemoryBytes},{r.Timestamp}");
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportToJson(IEnumerable<BenchmarkResult> results, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(results, options);
            File.WriteAllText(filePath, json);
        }
    }
}
