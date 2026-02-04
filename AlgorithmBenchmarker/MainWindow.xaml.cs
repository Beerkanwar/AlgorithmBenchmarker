using System.Windows;
using System.Windows.Media;
using AlgorithmBenchmarker.ViewModels;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AlgorithmBenchmarker
{
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = true;
        private readonly ResultsViewModel _resultsVM;

        public MainWindow(MainViewModel mainVM, ResultsViewModel resultsVM)
        {
            InitializeComponent();

            // DI Injected
            _resultsVM = resultsVM;
            var _mainVM = mainVM;

            // Wire up
            BenchmarkViewControl.DataContext = _mainVM;
            ResultsViewControl.DataContext = _resultsVM;

            // Listen for Tab Switching
            _mainVM.RequestResultsView += () =>
            {
                Dispatcher.Invoke(() => 
                {
                    MainTabControl.SelectedIndex = 1; // Index 1 is Results
                });
            };
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var res = Application.Current.Resources;

            if (_isDarkTheme)
            {
                res["BackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                res["SurfaceBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252526"));
                res["TextBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F3F46"));
                _resultsVM.LegendTextPaint = new SolidColorPaint(SKColors.White);
            }
            else
            {
                // Light Theme
                res["BackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9F9F9"));
                res["SurfaceBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                res["TextBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
                _resultsVM.LegendTextPaint = new SolidColorPaint(SKColors.Black);
            }
        }
    }
}