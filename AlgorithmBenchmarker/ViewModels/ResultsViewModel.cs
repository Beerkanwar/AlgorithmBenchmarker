using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AlgorithmBenchmarker.Data;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Microsoft.Win32;
using System.Collections.Generic;

namespace AlgorithmBenchmarker.ViewModels
{
    public class ResultsViewModel : ObservableObject
    {
        private readonly SQLiteRepository _repository;
        private readonly ExportService _exportService;

        public ResultsViewModel()
        {
            _repository = new SQLiteRepository();
            _exportService = new ExportService();
            _results = new ObservableCollection<BenchmarkResult>();
            _timeSeries = Array.Empty<ISeries>();
            _memorySeries = Array.Empty<ISeries>();
            
            LoadResultsCommand = new RelayCommand(LoadResults);
            ClearResultsCommand = new RelayCommand(ClearResults);
            ExportCsvCommand = new RelayCommand(ExportCsv);
            ExportJsonCommand = new RelayCommand(ExportJson);

            // Initial load
            LoadResults();
        }

        // Properties
        private ObservableCollection<BenchmarkResult> _results;
        public ObservableCollection<BenchmarkResult> Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        // Charts
        private ISeries[] _timeSeries;
        public ISeries[] TimeSeries
        {
            get => _timeSeries;
            set => SetProperty(ref _timeSeries, value);
        }

        private ISeries[] _memorySeries;
        public ISeries[] MemorySeries
        {
            get => _memorySeries;
            set => SetProperty(ref _memorySeries, value);
        }
        
        public Axis[] XAxes { get; set; } = new Axis[] { new Axis { Name = "Input Size", LabelsPaint = new SolidColorPaint(SKColors.White) } };
        public Axis[] YAxesTime { get; set; } = new Axis[] { new Axis { Name = "Time (ms)", LabelsPaint = new SolidColorPaint(SKColors.White) } };
        public Axis[] YAxesMem { get; set; } = new Axis[] { new Axis { Name = "Memory (Bytes)", LabelsPaint = new SolidColorPaint(SKColors.White) } };

        private SolidColorPaint _legendTextPaint = new SolidColorPaint { Color = SKColors.White };
        public SolidColorPaint LegendTextPaint
        {
            get => _legendTextPaint;
            set => SetProperty(ref _legendTextPaint, value);
        }


        // Commands
        public ICommand LoadResultsCommand { get; }
        public ICommand ClearResultsCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ExportJsonCommand { get; }
        
        private void ClearResults()
        {
            _repository.ClearAll();
            Results.Clear();
            TimeSeries = Array.Empty<ISeries>();
            MemorySeries = Array.Empty<ISeries>();
        }

        public void LoadResults()
        {
            var data = _repository.GetAllResults();
            Results = new ObservableCollection<BenchmarkResult>(data);
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            // Group by BatchId to find distinct runs
            // If BatchId is empty (legacy), group by Algo + Timestamp approx? No, just ignore or use Algo Name.
            
            // Logic: Find distinct batches. Plot each batch as a line.
            // If BatchId is missing, maybe fallback to grouping by AlgoName+Date?
            
            var batches = Results
                .GroupBy(r => !string.IsNullOrEmpty(r.BatchId) ? r.BatchId : r.Timestamp.ToString())
                .OrderBy(g => g.Max(r => r.Timestamp)) // Sort by latest batch first
                .Take(10); // Show last 10 batches only to keep chart clean

            var timeSeriesList = new ObservableCollection<ISeries>();
            var memSeriesList = new ObservableCollection<ISeries>();

            foreach (var batch in batches)
            {
                var runData = batch.OrderBy(r => r.InputSize).ToList();
                var seriesName = $"{runData.First().AlgorithmName} ({runData.First().Timestamp:HH:mm})";

                timeSeriesList.Add(new LineSeries<BenchmarkResult>
                {
                    Name = seriesName,
                    Values = runData,
                    Mapping = (r, index) => new LiveChartsCore.Kernel.Coordinate(r.InputSize, r.AvgTimeMs),
                    GeometrySize = 5,
                    LineSmoothness = 0.5
                });

                memSeriesList.Add(new LineSeries<BenchmarkResult> // Use Line for Memory too to show trend
                {
                    Name = seriesName,
                    Values = runData,
                    Mapping = (r, index) => new LiveChartsCore.Kernel.Coordinate(r.InputSize, r.MemoryBytes),
                    GeometrySize = 5
                });
            }

            TimeSeries = timeSeriesList.ToArray();
            MemorySeries = memSeriesList.ToArray();
        }

        private void ExportCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = "csv" };
            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportToCsv(Results, dialog.FileName);
            }
        }

        private void ExportJson()
        {
            var dialog = new SaveFileDialog { Filter = "JSON Files (*.json)|*.json", DefaultExt = "json" };
            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportToJson(Results, dialog.FileName);
            }
        }
    }
}
