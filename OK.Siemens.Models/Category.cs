using System.Security.AccessControl;

namespace OK.Siemens.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<PlcTag> Tags { get; set; } = new();
}