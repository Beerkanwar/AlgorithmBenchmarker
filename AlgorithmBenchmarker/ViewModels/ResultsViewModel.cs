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
using System;

namespace AlgorithmBenchmarker.ViewModels
{
    public class ResultsViewModel : ObservableObject
    {
        private readonly SQLiteRepository _repository;
        private readonly ExportService _exportService;

        public ResultsViewModel(SQLiteRepository repository)
        {
            _repository = repository;
            _exportService = new ExportService();
            _results = new ObservableCollection<BenchmarkResult>();
            _filteredResults = new ObservableCollection<BenchmarkResult>();
            _batchItems = new ObservableCollection<BatchItem>();
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

        private ObservableCollection<BenchmarkResult> _filteredResults;
        public ObservableCollection<BenchmarkResult> FilteredResults
        {
            get => _filteredResults;
            set => SetProperty(ref _filteredResults, value);
        }

        private ObservableCollection<BatchItem> _batchItems;
        public ObservableCollection<BatchItem> BatchItems
        {
            get => _batchItems;
            set => SetProperty(ref _batchItems, value);
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
        public Axis[] YAxesMem { get; set; } = new Axis[] { new Axis { Name = "Allocated (Bytes)", LabelsPaint = new SolidColorPaint(SKColors.White) } };

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
            FilteredResults.Clear();
            BatchItems.Clear();
            TimeSeries = Array.Empty<ISeries>();
            MemorySeries = Array.Empty<ISeries>();
        }

        public void LoadResults()
        {
            var data = _repository.GetAllResults();
            Results = new ObservableCollection<BenchmarkResult>(data);
            
            // Update BatchItems - Create selectable items for each algorithm batch
            var uniqueBatches = Results
                .GroupBy(r => r.BatchId)
                .Select(g => new BatchItem 
                { 
                    BatchId = g.Key, 
                    AlgorithmName = g.First().AlgorithmName,
                    IsSelected = false
                })
                .Where(b => !string.IsNullOrEmpty(b.BatchId))
                .OrderBy(b => b.AlgorithmName)
                .ToList();
            
            BatchItems = new ObservableCollection<BatchItem>(uniqueBatches);
            
            // Subscribe to changes in selection
            foreach (var item in BatchItems)
            {
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(BatchItem.IsSelected))
                    {
                        ApplyFilter();
                    }
                };
            }
            
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            // Get selected batch IDs
            var selectedBatchIds = BatchItems
                .Where(b => b.IsSelected)
                .Select(b => b.BatchId)
                .ToList();
            
            if (selectedBatchIds.Count == 0)
            {
                // No selection - show nothing
                FilteredResults = new ObservableCollection<BenchmarkResult>();
            }
            else
            {
                // Show results from selected batches
                FilteredResults = new ObservableCollection<BenchmarkResult>(
                    Results.Where(r => selectedBatchIds.Contains(r.BatchId)));
            }
            
            UpdateCharts();
        }

        private void UpdateCharts()
        {
            var source = FilteredResults;
            var batches = source
                .GroupBy(r => !string.IsNullOrEmpty(r.BatchId) ? r.BatchId : r.Timestamp.ToString())
                .OrderBy(g => g.Max(r => r.Timestamp))
                .Take(10); // Show last 10 batches only to keep chart clean if viewing all

            var timeSeriesList = new ObservableCollection<ISeries>();
            var memSeriesList = new ObservableCollection<ISeries>();

            foreach (var batch in batches)
            {
                var runData = batch.OrderBy(r => r.InputSize).ToList();
                var seriesName = $"{runData.First().AlgorithmName}";

                timeSeriesList.Add(new LineSeries<BenchmarkResult>
                {
                    Name = seriesName,
                    Values = runData,
                    Mapping = (r, index) => new LiveChartsCore.Kernel.Coordinate(r.InputSize, r.AvgTimeMs),
                    GeometrySize = 5,
                    LineSmoothness = 0.5
                });

                memSeriesList.Add(new LineSeries<BenchmarkResult> 
                {
                    Name = seriesName,
                    Values = runData,
                    Mapping = (r, index) => new LiveChartsCore.Kernel.Coordinate(r.InputSize, r.AllocatedBytes > 0 ? r.AllocatedBytes : r.MemoryBytes),
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
                _exportService.ExportToCsv(FilteredResults, dialog.FileName);
            }
        }

        private void ExportJson()
        {
            var dialog = new SaveFileDialog { Filter = "JSON Files (*.json)|*.json", DefaultExt = "json" };
            if (dialog.ShowDialog() == true)
            {
                _exportService.ExportToJson(FilteredResults, dialog.FileName);
            }
        }
    }
}
