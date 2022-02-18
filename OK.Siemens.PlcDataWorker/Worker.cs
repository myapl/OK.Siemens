using OK.Siemens.DataProviders;
using OK.Siemens.DataProviders.Interfaces;

namespace OK.Siemens.PlcDataWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPlcSiemensClient _client;

    public Worker(IPlcSiemensClient client, ILogger<Worker> logger)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();           
            }

            if (_client.IsConnected)
            {
                _client.Read();
            }
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(100, stoppingToken);
        }
    }
}