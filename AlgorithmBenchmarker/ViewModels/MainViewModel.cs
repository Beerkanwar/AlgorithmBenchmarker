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
        
        // Action to request tab switch
        public Action? RequestResultsView { get; set; }

        public MainViewModel(ResultsViewModel resultsVM)
        {
            _resultsViewModel = resultsVM;
            _registry = new AlgorithmRegistry();
            _runner = new BenchmarkRunner();
            _repository = new SQLiteRepository();

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
            
            RunBenchmarkCommand = new AsyncRelayCommand(RunBenchmarkAsync);
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

        private BenchmarkConfig _config;
        public BenchmarkConfig Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isResultReady;
        public bool IsResultReady
        {
            get => _isResultReady;
            set => SetProperty(ref _isResultReady, value);
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        public ObservableCollection<AlgorithmBenchmarker.Models.InputType> InputTypes { get; }
        public ObservableCollection<DistributionType> DistributionTypes { get; }


        // Commands
        public ICommand RunBenchmarkCommand { get; }
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
            // If algorithm is "Binary Search" (requires sorted input), force Sorted distribution
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
                    // Sorting algorithms are interesting to test on all distributions
                    if (!DistributionTypes.Contains(DistributionType.ReverseSorted))
                    {
                         DistributionTypes.Add(DistributionType.ReverseSorted);
                         DistributionTypes.Add(DistributionType.NearlySorted);
                    }
                }
            }
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
            StatusMessage = $"Running {SelectedAlgorithm.Name} (Range {Config.MinInputSize} - {Config.MaxInputSize})...";

            try
            {
                // Run Batch
                var results = await Task.Run(() => _runner.RunBatch(SelectedAlgorithm, Config));

                // Save All
                foreach (var r in results) _repository.SaveResult(r);
                
                StatusMessage = "Benchmark Complete! Click below to view results.";

                // Refresh Results
                _resultsViewModel.LoadResults();
                
                // Don't auto-switch, enable the button
                IsResultReady = true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                IsResultReady = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
