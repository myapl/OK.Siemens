using Microsoft.EntityFrameworkCore;
using OK.Siemens.DataProviders;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;
using OK.Siemens.PlcDataWorker;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.logs.json");
    })
    .ConfigureServices((hostBuilder, services) =>
    {
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(hostBuilder.Configuration["PgsqlConnectionString"]));
        services.AddScoped<IDataRecordsRepository, PgsqlDataRecordsRepository>();
        services.Configure<PlcParameters>(hostBuilder.Configuration.GetSection(PlcParameters.Parameters));
        services.AddSingleton<IPlcSiemensClient, PlcSiemensClient>();
        services.AddHostedService<Worker>();
    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        var appName = typeof(Program).Assembly.GetName().Name;
        if (appName != null)
            loggerConfiguration
                .ReadFrom.Configuration(hostContext.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", appName)
                .Enrich.WithProperty("Environment", hostContext.HostingEnvironment);
    })
    .Build();

await host.RunAsync();