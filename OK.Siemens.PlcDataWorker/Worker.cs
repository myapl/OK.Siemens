using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.PlcDataWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPlcSiemensClient _client;
    private readonly IHistoryService _historyService;
    private readonly IPlcDbParser _plcDbParser;

    public Worker(IPlcSiemensClient client, IHistoryService historyService, IPlcDbParser plcDbParser, ILogger<Worker> logger)
    {
        _logger = logger;
        _client = client;
        _historyService = historyService;
        _plcDbParser = plcDbParser;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dataRecords = new List<DataRecord>();

        await _historyService.AddTagsRangeAsync(_plcDbParser.ParseCsvFile("tags.csv"));
        
        var tagsList = await _historyService.GetTagsAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_client.IsConnected)
            {
                _client.Connect();           
            }
        
            if (_client.IsConnected)
            {
                var (error, tags) = _client.Read(20, tagsList.ToList());
                if (error == 0)
                {
                    dataRecords.Clear();
                    var receivedTime = DateTime.UtcNow;
                    foreach (var tag in tags)
                        dataRecords.Add(tag.ToDataRecord(receivedTime));

                    await _historyService.AddDataRecordsAsync(dataRecords);
                }
            }
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // await Task.Delay(300, stoppingToken);
        }
    }
}