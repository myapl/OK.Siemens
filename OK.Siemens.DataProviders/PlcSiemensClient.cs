namespace OK.Siemens.DataProviders;

public class PlcSiemensClient
{
    private readonly Sharp7 _plc;

    public PlcSiemensClient()
    {
        _plc = new Sharp7();
    }
}