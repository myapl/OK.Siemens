using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        //private readonly IConfiguration _configuration;
        
        // public App()
        // {
            // var builder = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            // _configuration = builder.Build();
        // }

        /// <summary>
        /// startup event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppStartup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            var serviceProvider = services.BuildServiceProvider();
            
            var mainWindowViewModel = serviceProvider.GetService<MainWindowViewModel>();
            
            
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<IHistoryService, HistoryService>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<MainWindow>();
        }
    }
}