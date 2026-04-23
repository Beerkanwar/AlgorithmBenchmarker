using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AlgorithmBenchmarker.Algorithms;
using AlgorithmBenchmarker.Data;
using AlgorithmBenchmarker.Models;
using AlgorithmBenchmarker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlgorithmBenchmarker.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly AlgorithmRegistry _registry;
        private readonly BenchmarkRunner _runner;
        private readonly SQLiteRepository _repository;
        private readonly ResultsViewModel _resultsViewModel;
        
        private CancellationTokenSource? _cts;
        
        // Action to request tab switch
        public Action? RequestResultsView { get; set; }

        public MainViewModel(ResultsViewModel resultsVM, AlgorithmRegistry registry, BenchmarkRunner runner, SQLiteRepository repository)
        {
            _resultsViewModel = resultsVM;
            _registry = registry;
            _runner = runner;
            _repository = repository;

            Categories = new ObservableCollection<string>(_registry.GetCategories());
            Config = new BenchmarkConfig();
            
            // Initialize Collections
            InputTypes = new ObservableCollection<AlgorithmBenchmarker.Models.InputType>(Enum.GetValues(typeof(AlgorithmBenchmarker.Models.InputType)).Cast<AlgorithmBenchmarker.Models.InputType>());
            DistributionTypes = new ObservableCollection<DistributionType>(Enum.GetValues(typeof(DistributionType)).Cast<DistributionType>());

            _selectedCategory = Categories.FirstOrDefault() ?? string.Empty;
            _algorithms = new ObservableCollection<IAlgorithm>();
            _selectedAlgorithm = null;
            _statusMessage = "Ready";

            UpdateAlgorithms();
            
            ExecutionModes = new ObservableCollection<string> 
            { 
                "Standard Batch Profiler", 
                "Drag Race Mode", 
                "Phase Transition Sweeper" 
            };
            _selectedExecutionMode = ExecutionModes.First();

            RunBenchmarkCommand = new AsyncRelayCommand(RunBenchmarkAsync, () => !IsBusy);
            CancelBenchmarkCommand = new RelayCommand(CancelBenchmark, () => IsBusy);
            ViewResultsCommand = new RelayCommand(ExecuteViewResults);
        }

        // Properties
        public ObservableCollection<string> Categories { get; }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                UpdateAlgorithms();
            }
        }

        public ObservableCollection<string> ExecutionModes { get; }

        private string _selectedExecutionMode;
        public string SelectedExecutionMode
        {
            get => _selectedExecutionMode;
            set => SetProperty(ref _selectedExecutionMode, value);
        }

        private ObservableCollection<IAlgorithm> _algorithms;
        public ObservableCollection<IAlgorithm> Algorithms
        {
            get => _algorithms;
            set => SetProperty(ref _algorithms, value);
        }

        private IAlgorithm? _selectedAlgorithm;
        public IAlgorithm? SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                if (SetProperty(ref _selectedAlgorithm, value))
                {
                    UpdateSettingsOptions();
                }
            }
        }

        private BenchmarkConfig _config = new BenchmarkConfig();
        public BenchmarkConfig Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    (RunBenchmarkCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
                    (CancelBenchmarkCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private bool _isResultReady;
        public bool IsResultReady
        {
            get => _isResultReady;
            set => SetProperty(ref _isResultReady, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        public ObservableCollection<AlgorithmBenchmarker.Models.InputType> InputTypes { get; }
        public ObservableCollection<DistributionType> DistributionTypes { get; }


        // Commands
        public ICommand RunBenchmarkCommand { get; }
        public ICommand CancelBenchmarkCommand { get; }
        public ICommand ViewResultsCommand { get; }

        private void ExecuteViewResults()
        {
            IsResultReady = false; // Reset status
            RequestResultsView?.Invoke();
        }

        private void UpdateAlgorithms()
        {
            if (string.IsNullOrEmpty(SelectedCategory)) return;
            Algorithms = new ObservableCollection<IAlgorithm>(_registry.GetAlgorithmsByCategory(SelectedCategory));
            SelectedAlgorithm = Algorithms.FirstOrDefault();
        }

        private void UpdateSettingsOptions()
        {
            // Reset to defaults
            if (DistributionTypes.Count != Enum.GetValues(typeof(DistributionType)).Length)
            {
                DistributionTypes.Clear();
                foreach (var d in Enum.GetValues(typeof(DistributionType)).Cast<DistributionType>())
                    DistributionTypes.Add(d);
            }

            // Example dynamic logic:
            if (SelectedAlgorithm != null)
            {
                if (SelectedAlgorithm.Category == "Searching" && SelectedAlgorithm.Name.Contains("Binary"))
                {
                    DistributionTypes.Clear();
                    DistributionTypes.Add(DistributionType.Sorted);
                    Config.Distribution = DistributionType.Sorted;
                }
                else if (SelectedAlgorithm.Category == "Sorting")
                {
                    if (!DistributionTypes.Contains(DistributionType.ReverseSorted))
                    {
                         DistributionTypes.Add(DistributionType.ReverseSorted);
                         DistributionTypes.Add(DistributionType.NearlySorted);
                    }
                }
            }
        }

        private void CancelBenchmark()
        {
            _cts?.Cancel();
        }

        private async Task RunBenchmarkAsync()
        {
            if (SelectedAlgorithm == null)
            {
                StatusMessage = "Please select an algorithm.";
                return;
            }

            IsBusy = true;
            IsResultReady = false;
            ProgressValue = 0;
            StatusMessage = $"Running {SelectedAlgorithm.Name} (Range {Config.MinInputSize} - {Config.MaxInputSize})...";
            
            _cts = new CancellationTokenSource();
            var progress = new Progress<int>(p => ProgressValue = p);

            try
            {
                if (SelectedExecutionMode == "Drag Race Mode")
                {
                    Config.EnableDragRaceMode = true;
                    var dragResults = await Task.Run(() => _runner.RunDragRace(Algorithms.ToList(), Config, _cts.Token));
                    if (!_cts.Token.IsCancellationRequested && dragResults.Any())
                    {
                        var msg = "Drag Race Results:\n\n";
                        foreach(var r in dragResults)
                            msg += $"Rank {r.Rank}: {r.AlgorithmName} ({r.ExecutionTimeMs:F4} ms | {r.AllocatedBytes:N0} bytes)\n";
                        System.Windows.MessageBox.Show(msg, "Drag Race Complete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        StatusMessage = "Drag Race Complete.";
                    }
                }
                else if (SelectedExecutionMode == "Phase Transition Sweeper")
                {
                    Config.EnablePhaseTransitionDetector = true;
                    var sweepResult = await Task.Run(() => _runner.RunPhaseTransitionSweep(SelectedAlgorithm, Config));
                    if (!_cts.Token.IsCancellationRequested)
                    {
                        var msg = $"Phase Transition Sweep Complete\n\nAlgorithm: {SelectedAlgorithm.Name}\nSweep Parameter: {Config.PhaseTransitionSweepParameter}\nCritical Parameter Detected (Exponent Inflection): {sweepResult.CriticalParameter:F4}\nMax Slope Delta: {sweepResult.MaxSlopeDelta:F4}";
                        System.Windows.MessageBox.Show(msg, "Phase Transition Detected", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        StatusMessage = "Phase Transition Sweep Complete.";
                    }
                }
                else
                {
                    // Run Standard Batch
                    var results = await Task.Run(() => _runner.RunBatch(SelectedAlgorithm, Config, _cts.Token, progress));

                    if (_cts.Token.IsCancellationRequested)
                    {
                        StatusMessage = "Benchmark Cancelled.";
                    }
                    else
                    {
                        // Save All
                        foreach (var r in results) _repository.SaveResult(r);
                        
                        StatusMessage = "Benchmark Complete! Click below to view results.";
                        IsResultReady = true;

                        // Refresh Results
                        _resultsViewModel.LoadResults();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                IsResultReady = false;
            }
            finally
            {
                IsBusy = false;
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}
