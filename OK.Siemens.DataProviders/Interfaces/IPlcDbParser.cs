using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Interfaces;

public interface IPlcDbParser
{
    /// <summary>
    /// Возвращает список тегов ПЛК из файла CSV
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    List<PlcTag> ParseCsvFile(string file);
}