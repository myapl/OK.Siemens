using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Migrations;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.WPF.TrendClient.ViewModels;

public class MainWindowViewModel: ObservableObject
{
    public ICommand OpenAddCategoryWindowCommand { get; }
    public ICommand CancelAddCategoryCommand { get; }
    public ICommand AddCategoryCommand { get; }
    public ICommand GetCategoriesCommand { get; }

    public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();

    private readonly IHistoryService _historyService;

    public MainWindowViewModel(IHistoryService historyService)
    {
        _historyService = historyService;
        OpenAddCategoryWindowCommand = new RelayCommand(OpenAddCategoryWindow);
        CancelAddCategoryCommand = new RelayCommand(CancelAddCategory);
        AddCategoryCommand = new AsyncRelayCommand(AddCategory);
        GetCategoriesCommand = new AsyncRelayCommand(GetCategories);
    }

    public void DialogClosing()
    {
        
    }

    private bool _isDialogAddCategoryVisible;
    public bool IsDialogAddCategoryVisible
    {
        get => _isDialogAddCategoryVisible;
        set => SetProperty(ref _isDialogAddCategoryVisible, value);
    }
    
    private bool _isCategoryLoading;
    public bool IsCategoryLoading
    {
        get => _isCategoryLoading;
        set => SetProperty(ref _isCategoryLoading, value);
    }

    private string _categoryName = "";
    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }
    
    private void OpenAddCategoryWindow()
    {
        IsDialogAddCategoryVisible = true;
    }

    private void CancelAddCategory()
    {
        IsDialogAddCategoryVisible = false;
    }

    private async Task AddCategory()
    {
        IsCategoryLoading = true;
        var(error, message) = await _historyService.AddCategoryAsync(_categoryName);
        if (!error)
            await GetCategories();
        CategoryName = "";
        await Task.Delay(200);
        IsCategoryLoading = false;
        IsDialogAddCategoryVisible = false;
    }

    private async Task GetCategories()
    {
        var(error, categories) = await _historyService.GetCategoriesAsync();
        if (!error)
        {
            Categories.Clear();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
        }
    }
}