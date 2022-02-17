using System.ComponentModel.DataAnnotations;

namespace OK.Siemens.Models;

public abstract class DataRecordBase
{
    public DateTime TimeStamp { get; set; }
    [MaxLength(64)]
    public string TagName { get; set; } = "";
}