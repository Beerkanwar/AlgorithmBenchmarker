using System.Windows;
using AlgorithmBenchmarker.ViewModels;

namespace AlgorithmBenchmarker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Wire up ViewModels
            var resultsVM = new ResultsViewModel();
            var mainVM = new MainViewModel(resultsVM);

            // Listen for Tab Switching
            mainVM.RequestResultsView += () =>
            {
                // Must run on UI thread
                Dispatcher.Invoke(() => 
                {
                    MainTabControl.SelectedIndex = 1; // Index 1 is Results
                });
            };

            BenchmarkViewControl.DataContext = mainVM;
            ResultsViewControl.DataContext = resultsVM;
        }
    }
}