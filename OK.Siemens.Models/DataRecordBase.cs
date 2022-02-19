using System.ComponentModel.DataAnnotations;

namespace OK.Siemens.Models;

public abstract class DataRecordBase
{
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public PlcTag TagName { get; set; } = new();
    // [MaxLength(64)] public string TagName { get; set; } = "";
}