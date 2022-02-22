using System.Security.AccessControl;

namespace OK.Siemens.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<PlcTag> Tags { get; set; } = new List<PlcTag>();
}