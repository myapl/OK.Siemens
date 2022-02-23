using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Collections;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.WPF.TrendClient.ViewModels;

public class MainWindowViewModel: ObservableObject
{
    public ObservableCollection<ISeries> Series { get; set; }
    
    public ICommand OpenAddCategoryWindowCommand { get; }
    public ICommand CancelAddCategoryCommand { get; }
    public ICommand AddCategoryCommand { get; }
    public ICommand GetCategoriesCommand { get; }
    public ICommand AddPenCommand { get; }
    public ICommand RemovePenCommand { get; }
    public ICommand SelectedItemChangedCommand { get; }

    public ObservableCollection<Category> Categories { get; private set; } = new ObservableCollection<Category>();
    public ObservableCollection<PenViewModel> PensCollection { get; set; } = new ObservableCollection<PenViewModel>(); 

    private readonly IHistoryService _historyService;
    private readonly BackgroundWorker _worker;

    private const string MOCK_DATA_NAME = "MockData";
    private LineSeries<DateTimePoint> _mockData = new()
    {
        // Name = MOCK_DATA_NAME,
        // Values = new List<DateTimePoint>
        // {
        //     new DateTimePoint(DateTime.UtcNow - TimeSpan.FromMinutes(120), 0),
        //     new DateTimePoint(DateTime.UtcNow, 0)
        // }
    };

    public RangeObservableCollection<DateTimePoint> ObservableValues { get; set; }
    public MainWindowViewModel(IHistoryService historyService)
    {
        _historyService = historyService;
        _worker = new BackgroundWorker();
        _worker.DoWork += PrepareTrendDataAsync;
        
        OpenAddCategoryWindowCommand = new RelayCommand(OpenAddCategoryWindow);
        CancelAddCategoryCommand = new RelayCommand(CancelAddCategory);
        AddCategoryCommand = new AsyncRelayCommand(AddCategory);
        GetCategoriesCommand = new AsyncRelayCommand(GetCategories);
        AddPenCommand = new AsyncRelayCommand<RoutedEventArgs>(AddPen);
        RemovePenCommand = new RelayCommand(RemovePen);
        SelectedItemChangedCommand = new RelayCommand<RoutedEventArgs>(SelectedItemChanged);

        ObservableValues = new RangeObservableCollection<DateTimePoint>();

        Series = new ObservableCollection<ISeries>()
        {
            //_mockData
        };
    }
    
    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            
            Labeler = value => new DateTime((long) value).ToString("hh:mm:ss"),
            LabelsRotation = 0,

            // in this case we want our columns with a width of 1 day, we can get that number
            // using the following syntax
            // UnitWidth = TimeSpan.FromMinutes(10).Ticks, // mark

            // The MinStep property forces the separator to be greater than 1 day.
            //MinStep = TimeSpan.FromMinutes(60).Ticks, // mark

            // if the difference between our points is in hours then we would:
            //UnitWidth = TimeSpan.FromSeconds(1).Ticks,

            // since all the months and years have a different number of days
            // we can use the average, it would not cause any visible error in the user interface
            // Months: TimeSpan.FromDays(30.4375).Ticks
            // Years: TimeSpan.FromDays(365.25).Ticks
            
        }
    };
    
    private int _chartItemsCount;
    public int ChartItemsCount
    {
        get => _chartItemsCount;
        set => SetProperty(ref _chartItemsCount, value);
    }
    
    private PlcTag? _selectedTag;
    public PlcTag? SelectedTag
    {
        get => _selectedTag;
        set => SetProperty(ref _selectedTag, value);
    }
    
    private PenViewModel? _selectedPen;
    public PenViewModel? SelectedPen
    {
        get => _selectedPen;
        set => SetProperty(ref _selectedPen, value);
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

    private DateTime _dateTimeStart;
    public DateTime DateTimeStart
    {
        get => _dateTimeStart;
        set => SetProperty(ref _dateTimeStart, value);
    }
    
    private DateTime _dateTimeEnd;
    public DateTime DateTimeEnd
    {
        get => _dateTimeEnd;
        set => SetProperty(ref _dateTimeEnd, value);
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

    private async Task AddPen(RoutedEventArgs? tag)
    {
        if (SelectedTag != null)
        {
            PensCollection.Add(new PenViewModel{Id = PensCollection.Count, Tag = SelectedTag});
            _worker.RunWorkerAsync();
        }
    }
    
    private void SelectedItemChanged(RoutedEventArgs? e)
    {
        if (e is not RoutedPropertyChangedEventArgs<object> {NewValue: PlcTag tag}) return;
        SelectedTag = tag;
    }

    private void RemovePen()
    {
        if (SelectedPen == null) return;
        var listToDelete = Series.FirstOrDefault(s => s.Name == SelectedPen.Tag.TagName);
        if (listToDelete != null) Series.Remove(listToDelete);
        PensCollection.Remove(SelectedPen);
        if (listToDelete != null)
            if (listToDelete.Values != null)
                ChartItemsCount -= listToDelete.Values.OfType<DateTimePoint>().Count();
        
        if (Series.Count == 0)
            Series.Add(_mockData);
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

    private async void PrepareTrendDataAsync(object? sender, DoWorkEventArgs e)
    {
        if (SelectedTag == null) return;
        var data = await _historyService.GetDataAsync(
            SelectedTag, DateTimeStart.ToUniversalTime(), DateTimeEnd.ToUniversalTime());

        var dataCollection = new ObservableCollection<DateTimePoint>();
        foreach (var dataRecord in data)
            dataCollection.Add(new DateTimePoint(dataRecord.TimeStamp, Math.Round((double)dataRecord.Value, 2)));

        var mockData = Series.FirstOrDefault(s => s.Name == MOCK_DATA_NAME);
        if (mockData != null)
            Series.Remove(mockData);

        var orderedData = dataCollection.OrderBy(d => d.DateTime).ToList();
        
        Series.Add(new LineSeries<DateTimePoint>{Values = dataCollection.OrderBy(d => d.DateTime), Name = SelectedTag.TagName, GeometryFill = null,
            GeometryStroke = null});

        ChartItemsCount += dataCollection.Count;
        
    }
}