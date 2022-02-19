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
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.DataRecords.AsNoTracking().Where(d => d.TimeStamp >= after && d.TimeStamp <= before).AsQueryable();
    }

    /// <summary>
    /// Add collection of records to repository
    /// </summary>
    /// <param name="dataRecords"></param>
    public async Task AddDataRecordsAsync(IEnumerable<DataRecord> dataRecords)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var enumerable = dataRecords.ToList();
        foreach (var entry in enumerable)
            dbContext.Entry(entry.TagName).State = EntityState.Unchanged;

        await dbContext.DataRecords.AddRangeAsync(enumerable);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Add plc tags collection to repository
    /// </summary>
    /// <param name="tags"></param>
    public async Task AddTagsAsync(IEnumerable<PlcTag> tags)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        foreach (var tag in tags)
        {
            var tagPresent = await dbContext.Tags.FirstOrDefaultAsync(t => t.Tagname == tag.Tagname);
            if (tagPresent == null)
            {
                await dbContext.AddAsync(tag);
            }
        }
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Return all plc tags from repository
    /// </summary>
    /// <returns></returns>
    public async Task<IQueryable<PlcTag>> GetTagsAsync()
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return dbContext.Tags.AsNoTracking().AsQueryable();
    }
}