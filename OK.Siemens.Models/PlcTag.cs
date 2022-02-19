using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OK.Siemens.Models;

public class PlcTag
{
    public int Id { get; set; }
    public string Tagname { get; set; } = "";
    public DataType DataType { get; set; }
    public DbAddress DbAddress { get; set; } = new();
    public string Description { get; set; } = "";
    [NotMapped] public object Data { get; set; } = "";
    
    public List<DataRecord> DataRecords { get; set; } = new();
    
    

    public static explicit operator DataRecord(PlcTag plcTag)
    {
        var item = new DataRecord
        {
            TagName = plcTag,
            TimeStamp = DateTime.UtcNow
        };

        plcTag.DataRecords.Add(item);

        switch (plcTag.DataType)
        {
            case DataType.Bool:
                item.Value = (bool) plcTag.Data ? 1.0f : 0.0f;
                break;
            case DataType.Word:
                throw new NotImplementedException();
            case DataType.DWord:
                throw new NotImplementedException();
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

[Owned] public class DbAddress
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