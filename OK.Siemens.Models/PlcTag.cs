using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace OK.Siemens.Models;

public class PlcTag
{
    public int Id { get; set; }
    public string TagName { get; set; } = "";
    public DataType DataType { get; set; }
    public DbAddress DbAddress { get; set; } = new();
    public string Description { get; set; } = "";
    [NotMapped] public object Data { get; set; } = "";

    public Category? Category { get; set; }
    
    public List<DataRecord> DataRecords { get; set; } = new();

    /// <summary>
    /// Convert Tag to Data record with a given DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public DataRecord ToDataRecord(DateTime dateTime)
    {
        if (dateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentOutOfRangeException(nameof(dateTime), "DateTime should be the Utc kind");
        
        var item = new DataRecord
        {
            TagName = this,
            TimeStamp = dateTime
        };

        switch (DataType)
        {
            case DataType.Bool:
                item.Value = (bool) Data ? 1.0f : 0.0f;
                break;
            case DataType.Word:
                throw new NotImplementedException();
            case DataType.DWord:
                throw new NotImplementedException();
            case DataType.UInt:
                item.Value = (ushort) Data;
                break;
            case DataType.Int:
                item.Value = (int) Data;
                break;
            case DataType.Real:
                item.Value = (float) Data;
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