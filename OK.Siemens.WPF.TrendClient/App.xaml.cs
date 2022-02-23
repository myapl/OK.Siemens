using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OK.Siemens.DataProviders;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.DataProviders.Services;
using OK.Siemens.WPF.TrendClient.ViewModels;

namespace OK.Siemens.WPF.TrendClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// startup event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppStartup(object sender, StartupEventArgs e)
        {
            var mainWindowViewModel = _serviceProvider.GetService<MainWindowViewModel>();
            if (mainWindowViewModel != null)
            {
                mainWindowViewModel.GetCategoriesCommand.Execute(null);
                DateTime.TryParse("2022-02-22 22:59:00", out var start);
                mainWindowViewModel.DateTimeStart = start;
                DateTime.TryParse("2022-02-22 23:01:00", out var end);
                mainWindowViewModel.DateTimeEnd = end;
            }

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContextFactory<AppDbContext>(options => 
                    options.UseNpgsql(_configuration["PgsqlConnectionString"]))
                .AddScoped<IDataRecordsRepository, PgsqlDataRecordsRepository>()
                .AddScoped<IHistoryService, HistoryService>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<MainWindow>();
        }
    }
}