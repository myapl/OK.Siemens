using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using OK.Siemens.DataProviders.Interfaces;
using OK.Siemens.Models;

namespace OK.Siemens.TrendsWpf.ViewModels;

public class MainWindowViewModel: ObservableObject
{
    
    private bool _receiveData;
    private int _query;
    private readonly BackgroundWorker _worker;
    private IQueryable<DataRecord> _collection;

    public ICommand StartReciveDataCommand { get; }
    public ICommand StopReciveDataCommand { get; }
    public ObservableCollection<ISeries> Series { get; set; }
    
    public MainWindowViewModel(IDataRecordsRepository repository)
    {
        _repository = repository;
        _worker = new BackgroundWorker();
        _worker.DoWork += SomeWork;
        
        StartReciveDataCommand = new AsyncRelayCommand(StartReceiveData);
        StopReciveDataCommand = new RelayCommand(StopReceiveData);
    }

    private async void SomeWork(object sender, DoWorkEventArgs e)
    {
        var tags = new List<PlcTag>
        {
            new PlcTag{Id = 1},
            new PlcTag{Id = 2},
            new PlcTag{Id = 3},
            new PlcTag{Id = 4}
        };
        _receiveData = true;
        var count = 0;
        var stopWatch = new Stopwatch();
        while (_receiveData)
        {
            stopWatch.Reset();
            stopWatch.Start();
            _query++;
            var after = DateTime.UtcNow - TimeSpan.FromMinutes(10);
            _collection = await _repository.GetRecordsBetweenTime(after, DateTime.UtcNow);
            
            // var newCollection = _collection.Where(x => tags.Contains(x.TagName));
            // count = newCollection.Count();
            count = _collection.Count();
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Count = $"Query: {_query} Count: {count} Time: {elapsedTime}";

        }
    }

    private string? _count;
    private readonly IDataRecordsRepository _repository;

    public string? Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    private async Task StartReceiveData()
    {
        _worker.RunWorkerAsync();
    }

    private void StopReceiveData()
    {
        _receiveData = false;
    }
}