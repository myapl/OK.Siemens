namespace OK.Siemens.Models;

public class PlcParameters
{
    public const string Parameters = "PlcParameters";
    public string Address { get; set; } = "";
    public int Rack { get; set; }
    public int Slot { get; set; }
}