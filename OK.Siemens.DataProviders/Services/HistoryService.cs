using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Services;

public class HistoryService : IHistoryService
{
    private readonly IDataRecordsRepository _repository;

    public HistoryService(IDataRecordsRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>
    /// Add tags category to repository
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public async Task<(bool error, string message)> AddCategoryAsync(string categoryName)
    {
        return await _repository.AddCategoryAsync(new Category{Name = categoryName});
    }

    /// <summary>
    /// Get tags categories
    /// </summary>
    /// <returns></returns>
    public async Task<(bool error, IQueryable<Category>?)> GetCategoriesAsync()
    {
        return await _repository.GetCategoriesAsync();
    }

    /// <summary>
    /// Return list of all plc tags
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<PlcTag>> GetTagsAsync()
    {
        return await _repository.GetTagsAsync();
    }
}