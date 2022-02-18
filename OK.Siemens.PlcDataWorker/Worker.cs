using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.PlcDataWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPlcSiemensClient _client;
    private readonly IDataRecordsRepository _repository;
    private readonly IPlcDbParser _plcDbParser;

    public Worker(IPlcSiemensClient client, IDataRecordsRepository repository, IPlcDbParser plcDbParser, ILogger<Worker> logger)
    {
        _logger = logger;
        _client = client;
        _repository = repository;
        _plcDbParser = plcDbParser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tagsList = _plcDbParser.ParseCsvFile("tags.csv");
        var dataRecords = new List<DataRecord>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();           
            }
        
            if (_client.IsConnected)
            {
                var (error, tags) = _client.Read(20, tagsList);

                if (error == 0)
                {
                    dataRecords.Clear();
                    foreach (var tag in tags)
                    {
                        //if (dataRecords.Count < 2)
                            dataRecords.Add((DataRecord)tag);
                    }
                    await _repository.AddDataRecordsAsync(dataRecords);
                }
            }
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //await Task.Delay(500, stoppingToken);
        }
    }
}