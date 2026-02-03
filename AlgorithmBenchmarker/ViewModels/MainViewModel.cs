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
            _selectedCategory = Categories.FirstOrDefault() ?? string.Empty;
            _algorithms = new ObservableCollection<IAlgorithm>();
            _selectedAlgorithm = null;
            _statusMessage = "Ready";

            UpdateAlgorithms();
            RunBenchmarkCommand = new AsyncRelayCommand(RunBenchmarkAsync);
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
            set => SetProperty(ref _selectedAlgorithm, value);
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

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        public IEnumerable<AlgorithmBenchmarker.Models.InputType> InputTypes => Enum.GetValues(typeof(AlgorithmBenchmarker.Models.InputType)).Cast<AlgorithmBenchmarker.Models.InputType>();
        public IEnumerable<DistributionType> DistributionTypes => Enum.GetValues(typeof(DistributionType)).Cast<DistributionType>();


        // Commands
        public ICommand RunBenchmarkCommand { get; }

        private void UpdateAlgorithms()
        {
            if (string.IsNullOrEmpty(SelectedCategory)) return;
            Algorithms = new ObservableCollection<IAlgorithm>(_registry.GetAlgorithmsByCategory(SelectedCategory));
            SelectedAlgorithm = Algorithms.FirstOrDefault();
        }

        private async Task RunBenchmarkAsync()
        {
            if (SelectedAlgorithm == null)
            {
                StatusMessage = "Please select an algorithm.";
                return;
            }

            IsBusy = true;
            StatusMessage = $"Running {SelectedAlgorithm.Name} (Range {Config.MinInputSize} - {Config.MaxInputSize})...";

            try
            {
                // Run Batch
                var results = await Task.Run(() => _runner.RunBatch(SelectedAlgorithm, Config));

                // Save All
                foreach (var r in results) _repository.SaveResult(r);
                
                StatusMessage = "Benchmark Complete!";

                // Refresh Results
                _resultsViewModel.LoadResults();
                
                // Auto Switch
                RequestResultsView?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
