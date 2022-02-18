using OK.Siemens.DataProviders;
using OK.Siemens.DataProviders.Interfaces;

namespace OK.Siemens.PlcDataWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPlcSiemensClient _client;
    private readonly IDataRecordsRepository _repository;

    public Worker(IPlcSiemensClient client, IDataRecordsRepository repository, ILogger<Worker> logger)
    {
        _logger = logger;
        _client = client;
        _repository = repository;
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
                var error = _client.Read();
            }
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(100, stoppingToken);
        }
    }
}