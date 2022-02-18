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

    public (int error, IEnumerable<PlcTag> tags) Read(int dbNumber, IList<PlcTag> tagsList)
    {
        const int bufferSize = 10240;
        var data = new byte[bufferSize];
        var readDataSize = 0;
        var error = _client.ReadArea(S7Consts.S7AreaDB, dbNumber, 0, CalculateBytesAmount(tagsList.Last(), bufferSize), S7Consts.S7WLByte, data, ref readDataSize);

        if (error == 0)
        {
            _logger.LogDebug("Success reading from PLC [{address}] data of {readDataSize} bytes", _plcParameters.Address, readDataSize);
            foreach (var tag in tagsList)
            {
                switch (tag.DataType)
                {
                    case DataType.Bool:
                        tag.Data = S7.GetBitAt(data, tag.DbAddress.Byte, tag.DbAddress.Bit);
                        break;
                    case DataType.Word:
                        break;
                    case DataType.DWord:
                        break;
                    case DataType.UInt:
                        tag.Data = S7.GetUIntAt(data, tag.DbAddress.Byte);
                        break;
                    case DataType.Int:
                        break;
                    case DataType.Real:
                        tag.Data = S7.GetRealAt(data, tag.DbAddress.Byte);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else
            _logger.LogWarning("Error reading data from PLC [{address}], error [{error}]", _plcParameters.Address, error);
        
        return (error, tagsList);
    }
    
    public void Dispose()
    {
        _logger.LogInformation("PLC disconnecting...");
        _client.Disconnect();
        _logger.LogInformation("Disconnected");
    }

    private int CalculateBytesAmount(PlcTag lastTag, int bufferSize)
    {
        switch (lastTag.DataType)
        {
            case DataType.Bool: case DataType.Word: case DataType.UInt: case DataType.Int:
                if (lastTag.DbAddress.Byte + 2 > bufferSize)
                    throw new ArgumentOutOfRangeException(nameof(lastTag), 
                        $"Reading data size is more then buffer {bufferSize} bytes size");
                return lastTag.DbAddress.Byte + 2;
            
            case DataType.DWord: case DataType.Real:
                if (lastTag.DbAddress.Byte + 4 > bufferSize)
                    throw new ArgumentOutOfRangeException(nameof(lastTag), 
                        $"Reading data size is more then buffer {bufferSize} bytes size");
                return lastTag.DbAddress.Byte + 4;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(lastTag.DataType), 
                    "Can't calculate amount of reading data for this DataType");
        }
    }
}