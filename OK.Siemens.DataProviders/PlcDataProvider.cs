namespace OK.Siemens.DataProviders;

public class PlcDataProvider
{
    private readonly Sharp7 _plc;

    public PlcDataProvider()
    {
        _plc = new Sharp7();
    }
}