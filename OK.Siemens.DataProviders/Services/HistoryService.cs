using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Add tags to repository
    /// If category == null put tags to category "root"
    /// </summary>
    /// <param name="tags"></param>
    /// <returns>true in error occured</returns>
    public async Task<bool> AddTagsRangeAsync(IEnumerable<PlcTag> tags)
    {
        try
        {
            var rootCategory = await GetRootCategory();
            if (rootCategory == null)
                return true;
            var tagsList = tags.ToList();
            foreach (var tag in tagsList)
                tag.Category ??= rootCategory;
            await _repository.AddTagsAsync(tagsList);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }

    /// <summary>
    /// Add data records to repository
    /// </summary>
    /// <param name="dataRecords"></param>
    /// <returns></returns>
    public async Task AddDataRecordsAsync(IEnumerable<DataRecord> dataRecords)
    {
        await _repository.AddDataRecordsAsync(dataRecords);
    }

    private async Task<Category?> GetRootCategory()
    {
        var (error, categories) = await _repository.GetCategoriesAsync();
        if (error || categories == null)
            return null;
        var rootCategory = await categories.FirstOrDefaultAsync(c => c.Id == 1);
        if (rootCategory != null) return rootCategory;
        {
            (error, _) = await _repository.AddCategoryAsync(new Category {Id = 1, Name = "root"});
            if (error)
                return null;
            (error, categories) = await _repository.GetCategoriesAsync();
            if (error || categories == null)
                return null;
            rootCategory = await categories.FirstOrDefaultAsync(c => c.Id == 1);
            
            return rootCategory ?? null;
        }
    }
}