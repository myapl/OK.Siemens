namespace OK.Siemens.Models;

public class PlcTag
{
    public string Tagname { get; set; } = "";
    public DataType DataType { get; set; }
    public DbAddress DbAddress { get; set; } = new();
    public string Description { get; set; } = "";
    public object Data { get; set; } = "";

    public static explicit operator DataRecord(PlcTag plcTag)
    {
        var item = new DataRecord
        {
            TagName = plcTag.Tagname,
            TimeStamp = DateTime.UtcNow
        };

        switch (plcTag.DataType)
        {
            case DataType.Bool:
                item.Value = (bool) plcTag.Data ? 1.0f : 0.0f;
                break;
            case DataType.Word:
                throw new NotImplementedException();
                break;
            case DataType.DWord:
                throw new NotImplementedException();
                break;
            case DataType.UInt:
                item.Value = (ushort) plcTag.Data;
                break;
            case DataType.Int:
                item.Value = (int) plcTag.Data;
                break;
            case DataType.Real:
                item.Value = (float) plcTag.Data;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return item;
    }
}

public class DbAddress
{
    public int Byte { get; set; }
    public int Bit { get; set; }
}

public enum DataType
{
    Bool,
    Word,
    DWord,
    UInt,
    Int,
    Real,
} 