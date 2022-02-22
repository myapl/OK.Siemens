using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Interfaces;

public interface IDataRecordsRepository
{
    /// <summary>
    /// Select data records between after and before
    /// </summary>
    /// <param name="after"></param>
    /// <param name="before"></param>
    /// <returns></returns>
    Task<IQueryable<DataRecord>> GetRecordsBetweenTime(DateTime after, DateTime before); // IEnumerable<PlcTag> tags

    /// <summary>
    /// Add collection of records to repository
    /// </summary>
    /// <param name="dataRecords"></param>
    Task AddDataRecordsAsync(IEnumerable<DataRecord> dataRecords);

    /// <summary>
    /// Add plc tags collection to repository
    /// </summary>
    /// <param name="tags"></param>
    Task AddTagsAsync(IEnumerable<PlcTag> tags);

    /// <summary>
    /// Return all plc tags from repository
    /// </summary>
    /// <returns></returns>
    Task<IQueryable<PlcTag>> GetTagsAsync();

    /// <summary>
    /// Add new category
    /// </summary>
    /// <param name="category"></param>
    /// <returns>True if error occured</returns>
    Task<(bool error, string message)> AddCategoryAsync(Category category);
    
    /// <summary>
    /// Edit category
    /// </summary>
    /// <param name="category"></param>
    /// <returns>True if error occured</returns>
    Task<bool> EditCategoryAsync(Category category);

    /// <summary>
    /// Return list of categories
    /// </summary>
    /// <returns></returns>
    Task<(bool error, IQueryable<Category>? categories)> GetCategoriesAsync();

    Task<bool> UpdateTag(PlcTag tag);
}