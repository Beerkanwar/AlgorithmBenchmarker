using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using AlgorithmBenchmarker.Models;
using System.IO;
using System.Text.Json;

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
                
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Results (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        BatchId TEXT,
                        AlgorithmName TEXT,
                        Category TEXT,
                        InputSize INTEGER,
                        AvgTimeMs REAL,
                        MinTimeMs REAL DEFAULT 0,
                        MaxTimeMs REAL DEFAULT 0,
                        StdDevTimeMs REAL DEFAULT 0,
                        AllocatedBytes INTEGER DEFAULT 0,
                        MemoryBytes INTEGER,
                        Timestamp TEXT
                    );
                ";
                command.ExecuteNonQuery();
                
                // Migrations
                AddColumnIfNotExists(connection, "BatchId", "TEXT DEFAULT ''");
                AddColumnIfNotExists(connection, "MinTimeMs", "REAL DEFAULT 0");
                AddColumnIfNotExists(connection, "MaxTimeMs", "REAL DEFAULT 0");
                AddColumnIfNotExists(connection, "StdDevTimeMs", "REAL DEFAULT 0");
                AddColumnIfNotExists(connection, "AllocatedBytes", "INTEGER DEFAULT 0");
                AddColumnIfNotExists(connection, "ExtendedMetrics", "TEXT DEFAULT '{}'");
            }
        }

        private void AddColumnIfNotExists(SqliteConnection connection, string columnName, string typeDef)
        {
            try 
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"ALTER TABLE Results ADD COLUMN {columnName} {typeDef}";
                cmd.ExecuteNonQuery();
            }
            catch { /* Column exists */ }
        }

        public void SaveResult(BenchmarkResult result)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Results (BatchId, AlgorithmName, Category, InputSize, AvgTimeMs, MinTimeMs, MaxTimeMs, StdDevTimeMs, AllocatedBytes, MemoryBytes, Timestamp, ExtendedMetrics)
                    VALUES ($batch, $name, $cat, $size, $time, $min, $max, $std, $alloc, $mem, $date, $extended);
                ";
                command.Parameters.AddWithValue("$batch", result.BatchId ?? "");
                command.Parameters.AddWithValue("$name", result.AlgorithmName);
                command.Parameters.AddWithValue("$cat", result.Category);
                command.Parameters.AddWithValue("$size", result.InputSize);
                command.Parameters.AddWithValue("$time", result.AvgTimeMs);
                command.Parameters.AddWithValue("$min", result.MinTimeMs);
                command.Parameters.AddWithValue("$max", result.MaxTimeMs);
                command.Parameters.AddWithValue("$std", result.StdDevTimeMs);
                command.Parameters.AddWithValue("$alloc", result.AllocatedBytes);
                command.Parameters.AddWithValue("$mem", result.MemoryBytes);
                command.Parameters.AddWithValue("$date", result.Timestamp.ToString("o"));
                string jsonMetrics = JsonSerializer.Serialize(result.ExtendedMetrics ?? new Dictionary<string, string>());
                command.Parameters.AddWithValue("$extended", jsonMetrics);

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
                        var result = new BenchmarkResult
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AlgorithmName = reader.GetString(reader.GetOrdinal("AlgorithmName")),
                            Category = reader.GetString(reader.GetOrdinal("Category")),
                            InputSize = reader.GetInt32(reader.GetOrdinal("InputSize")),
                            AvgTimeMs = reader.GetDouble(reader.GetOrdinal("AvgTimeMs")),
                            MemoryBytes = reader.GetInt64(reader.GetOrdinal("MemoryBytes")),
                            Timestamp = DateTime.Parse(reader.GetString(reader.GetOrdinal("Timestamp")))
                        };

                        // Optional Columns (handle if old DB without them, though Initialize ensures they exist if we ran it)
                        // But GetOrdinal throws if not found? No, returns -1? No, throws.
                        // InitializeDatabase ensures they exist.
                        
                        // Handle potential DBNull or defaults
                        int batchIdx = reader.GetOrdinal("BatchId");
                        result.BatchId = reader.IsDBNull(batchIdx) ? "" : reader.GetString(batchIdx);

                        int minIdx = reader.GetOrdinal("MinTimeMs");
                        result.MinTimeMs = reader.IsDBNull(minIdx) ? 0 : reader.GetDouble(minIdx);
                        
                        int maxIdx = reader.GetOrdinal("MaxTimeMs");
                        result.MaxTimeMs = reader.IsDBNull(maxIdx) ? 0 : reader.GetDouble(maxIdx);

                        int stdIdx = reader.GetOrdinal("StdDevTimeMs");
                        result.StdDevTimeMs = reader.IsDBNull(stdIdx) ? 0 : reader.GetDouble(stdIdx);

                        int allocIdx = reader.GetOrdinal("AllocatedBytes");
                        result.AllocatedBytes = reader.IsDBNull(allocIdx) ? 0 : reader.GetInt64(allocIdx);

                        try
                        {
                            int extIdx = reader.GetOrdinal("ExtendedMetrics");
                            string extJson = reader.IsDBNull(extIdx) ? "{}" : reader.GetString(extIdx);
                            result.ExtendedMetrics = JsonSerializer.Deserialize<Dictionary<string, string>>(extJson) ?? new Dictionary<string, string>();
                        }
                        catch { result.ExtendedMetrics = new Dictionary<string, string>(); }

                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public void ClearAll()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Results";
                command.ExecuteNonQuery();
            }
        }
    }
}
