namespace OK.Siemens.DataProviders.Interfaces;

public interface IPlcSiemensClient
{
    public bool IsConnected { get; }
    int Connect();
    int Read();
}