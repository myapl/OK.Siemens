using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Interfaces;

public interface IDataRecordsRepository
{
    /// <summary>
    /// Select data records between after and before
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    Task<IQueryable<DataRecord>> GetRecordsBetweenTime(DateTime after, DateTime before);

    /// <summary>
    /// Add collection of records to repository
    /// </summary>
    /// <param name="dataRecords"></param>
    Task AddDataRecordsAsync(IEnumerable<DataRecord> dataRecords);
}