using Microsoft.EntityFrameworkCore;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.DataProviders;

public class PgsqlDataRecordsRepository : IDataRecordsRepository
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    
    public PgsqlDataRecordsRepository(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    /// <summary>
    /// Select data records between after and before
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    public async Task<IQueryable<DataRecord>> GetRecordsBetweenTime(DateTime after, DateTime before)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.DataRecords.Where(d => d.TimeStamp >= after && d.TimeStamp <= before).AsQueryable();
    }

    /// <summary>
    /// Add collection of records to repository
    /// </summary>
    /// <param name="dataRecords"></param>
    public async Task AddDataRecordsAsync(IEnumerable<DataRecord> dataRecords)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        await dbContext.AddRangeAsync(dataRecords);
        await dbContext.SaveChangesAsync();
    }
}