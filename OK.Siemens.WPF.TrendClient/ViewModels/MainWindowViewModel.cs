using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OK.Siemens.WPF.TrendClient.ViewModels;

public class MainWindowViewModel: ObservableObject
{
    public ICommand OpenAddCategoryWindowCommand { get; }
    public ICommand CancelAddCategoryCommand { get; }

    private bool _isAddCategoryModalVisible;

    public MainWindowViewModel()
    {
        OpenAddCategoryWindowCommand = new RelayCommand(OpenAddCategoryWindow);
        CancelAddCategoryCommand = new RelayCommand(CancelAddCategory);
    }

    public bool IsAddCategoryModalVisible
    {
        get => _isAddCategoryModalVisible;
        set => SetProperty(ref _isAddCategoryModalVisible, value);
    }
    
    private void OpenAddCategoryWindow()
    {
        IsAddCategoryModalVisible = true;
    }

    private void CancelAddCategory()
    {
        IsAddCategoryModalVisible = false;
    }
}