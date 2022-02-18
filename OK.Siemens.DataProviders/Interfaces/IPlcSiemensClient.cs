using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Interfaces;

public interface IPlcSiemensClient
{
    public bool IsConnected { get; }
    int Connect();
    (int error, IEnumerable<PlcTag> tags) Read(int dbNumber, IList<PlcTag> tagsList);
}