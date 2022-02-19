using Microsoft.VisualBasic.FileIO;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;
using static System.Int32;

namespace OK.Siemens.DataProviders;

public class PlcDbParser : IPlcDbParser
{
    /// <summary>
    /// Возвращает список тегов ПЛК из файла CSV
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public List<PlcTag> ParseCsvFile(string file)
    {
        var tagsList = new List<PlcTag>();
        
        using (var parser = new TextFieldParser(file))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");
            
            var tag = new PlcTag();
            var fieldNumber = 0;
            
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields() ?? Array.Empty<string>();
                foreach (var field in fields)
                {
                    switch (fieldNumber)
                    {
                        case 1:
                            tag.Tagname = field;
                            break;
                        case 2:
                            Enum.TryParse<DataType>(field, out var dataType);
                            tag.DataType = dataType;
                            break;
                        case 3:
                            tag.DbAddress = SplitDbAddress(field, tag.Tagname);
                            break;
                        case 11:
                            tag.Description = field;
                            break;
                    }
                    
                    fieldNumber++;
                    if (fieldNumber <= 11) continue;
                    
                    fieldNumber = 0;
                    tagsList.Add(tag);
                    tag = new PlcTag();
                }
            }
        }

        return tagsList;
    }

    private DbAddress SplitDbAddress(string field, string tagname)
    {
        var splintered = field.Split(".");
        _ = TryParse(splintered[0], out var addressByte);
        _ = TryParse(splintered[1], out var addressBit);
        return new DbAddress{Bit = addressBit, Byte = addressByte}; //, Tagname = tagname};
    }
}