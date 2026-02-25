using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlgorithmBenchmarker.Services;
using AlgorithmBenchmarker.ViewModels;
using AlgorithmBenchmarker.Data;

namespace AlgorithmBenchmarker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<AlgorithmRegistry>();
            services.AddSingleton<BenchmarkRunner>();
            services.AddSingleton<SQLiteRepository>();

            // ViewModels
            services.AddSingleton<ResultsViewModel>();
            services.AddTransient<MainViewModel>();

            // Views
            services.AddTransient<MainWindow>();
        }
    }

}
