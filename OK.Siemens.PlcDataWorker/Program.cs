using OK.Siemens.PlcDataWorker;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.logs.json");
    })
    .ConfigureServices(services =>
    {
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