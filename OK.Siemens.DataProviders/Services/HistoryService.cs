using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.DataProviders.Services;

public class HistoryService
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
}