using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;
using static OK.Siemens.DataProviders.Sharp7;

namespace OK.Siemens.DataProviders;

public class PlcSiemensClient: IDisposable, IPlcSiemensClient
{
    private readonly S7Client _client;
    private readonly PlcParameters _plcParameters;
    private readonly ILogger<PlcSiemensClient> _logger;
    
    public bool IsConnected { get => _client.Connected; }

    public PlcSiemensClient(IOptions<PlcParameters> plcParameters, ILogger<PlcSiemensClient> logger)
    {
        _client = new S7Client();
        _plcParameters = plcParameters.Value;
        _logger = logger;
    }

    public int Connect()
    {
        var error = _client.ConnectTo(_plcParameters.Address, _plcParameters.Rack, _plcParameters.Slot);
        if (error == 0)
            _logger.LogInformation("Connected to PLC [{address}]", _plcParameters.Address);
        else
            _logger.LogWarning("Error connecting to PLC [{address}], error [{error}]", _plcParameters.Address, error);
        return error;
    }

    public int Read()
    {
        var data = new byte[1024];
        var error = _client.ReadArea(S7Consts.S7AreaDB, 2, 0, 4, S7Consts.S7WLReal, data);
        if (error == 0)
            _logger.LogInformation("Success reading data from PLC [{address}]", _plcParameters.Address);
        else
            _logger.LogWarning("Error reading data from PLC [{address}], error [{error}]", _plcParameters.Address, error);
        return error;
    }
    
    public void Dispose()
    {
        _logger.LogInformation("PLC disconnecting...");
        _client.Disconnect();
        _logger.LogInformation("Disconnected");
    }
}