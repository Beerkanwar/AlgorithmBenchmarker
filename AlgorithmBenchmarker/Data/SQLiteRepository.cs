using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using AlgorithmBenchmarker.Models;
using System.IO;

namespace AlgorithmBenchmarker.Data
{
    public class SQLiteRepository
    {
        private string _dbPath;
        private string ConnectionString => $"Data Source={_dbPath}";

        public SQLiteRepository()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(folder, "AlgorithmBenchmarker");
            Directory.CreateDirectory(path);
            _dbPath = Path.Combine(path, "benchmarks.db");

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // Add BatchId. If modifying existing DB, we might need manual migration or just drop/recreate for a student project.
                // Simpler for dev: Use IF NOT EXISTS and hope, but to force update for new column, 
                // we can catch error or just DROP TABLE (destructive).
                // Let's go with "Create with BatchId". If table exists without it, it might fail or ignore.
                // Ideally: ALTER TABLE ADD COLUMN.
                
                // Hack for student project: Try adding column if missing, or just rename table/recreate. e.g. "ResultsV2"
                // Or: Just add the column via ExecuteNonQuery ignoring errors.

                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Results (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        BatchId TEXT,
                        AlgorithmName TEXT,
                        Category TEXT,
                        InputSize INTEGER,
                        AvgTimeMs REAL,
                        MemoryBytes INTEGER,
                        Timestamp TEXT
                    );
                ";
                command.ExecuteNonQuery();
                
                // Attempt to add column if it doesn't exist (harmless failure if exists usually, or check schema)
                // SQLite doesn't have "IF NOT EXISTS" for columns easily.
                // Let's just catch.
                try 
                {
                    var alterCmd = connection.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE Results ADD COLUMN BatchId TEXT DEFAULT ''";
                    alterCmd.ExecuteNonQuery();
                }
                catch { /* Column likely exists */ }
            }
        }

        public void SaveResult(BenchmarkResult result)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Results (BatchId, AlgorithmName, Category, InputSize, AvgTimeMs, MemoryBytes, Timestamp)
                    VALUES ($batch, $name, $cat, $size, $time, $mem, $date);
                ";
                command.Parameters.AddWithValue("$batch", result.BatchId ?? "");
                command.Parameters.AddWithValue("$name", result.AlgorithmName);
                command.Parameters.AddWithValue("$cat", result.Category);
                command.Parameters.AddWithValue("$size", result.InputSize);
                command.Parameters.AddWithValue("$time", result.AvgTimeMs);
                command.Parameters.AddWithValue("$mem", result.MemoryBytes);
                command.Parameters.AddWithValue("$date", result.Timestamp.ToString("o"));

                command.ExecuteNonQuery();
            }
        }

        public List<BenchmarkResult> GetAllResults()
        {
            var results = new List<BenchmarkResult>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Results ORDER BY Timestamp DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var batchIdOrdinal = reader.GetOrdinal("BatchId");
                        var algoNameOrdinal = reader.GetOrdinal("AlgorithmName");
                        var categoryOrdinal = reader.GetOrdinal("Category");
                        var inputSizeOrdinal = reader.GetOrdinal("InputSize");
                        var timeOrdinal = reader.GetOrdinal("AvgTimeMs");
                        var memOrdinal = reader.GetOrdinal("MemoryBytes");
                        var timestampOrdinal = reader.GetOrdinal("Timestamp");

                        results.Add(new BenchmarkResult
                        {
                            Id = reader.GetInt32(0), // Id is usually 0
                            BatchId = reader.IsDBNull(batchIdOrdinal) ? "" : reader.GetString(batchIdOrdinal),
                            AlgorithmName = reader.GetString(algoNameOrdinal),
                            Category = reader.GetString(categoryOrdinal),
                            InputSize = reader.GetInt32(inputSizeOrdinal),
                            AvgTimeMs = reader.GetDouble(timeOrdinal),
                            MemoryBytes = reader.GetInt64(memOrdinal),
                            Timestamp = DateTime.Parse(reader.GetString(timestampOrdinal))
                        });
                    }
                }
            }
            return results;
        }
    }
}
