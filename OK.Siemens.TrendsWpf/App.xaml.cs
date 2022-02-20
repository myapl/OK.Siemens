using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OK.Siemens.DataProviders;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.TrendsWpf.ViewModels;

namespace OK.Siemens.TrendsWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, configuration) =>
                {
                    configuration.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.AddDbContextFactory<AppDbContext>(options =>
                    {
                        if (ctx.Configuration != null)
                            options.UseNpgsql(ctx.Configuration["PgsqlConnectionString"]);
                    });
                    services.AddSingleton<IDataRecordsRepository, PgsqlDataRecordsRepository>();
                    services.AddScoped<MainWindowViewModel>();
                    services.AddScoped<MainWindow>();
                })
                .Build();
        }
        
        //public IConfiguration? Configuration { get; private set; }
        protected async void AppStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow?.Show();
            

            // var builder = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            // Configuration = builder.Build();

            //var hostBuilder = new HostBuilder().Build();

            // var services = new ServiceCollection();
            // ConfigureServices(services);
            // var serviceProvider = services.BuildServiceProvider();
            // var mainWindow = serviceProvider.GetService<MainWindow>();
            // mainWindow?.Show();
        }
        
    }
}