
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Persistence;
using RaSetMaker.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaSetMaker.ViewModels;

public partial class RomSetGeneratorDialogViewModel : ViewModelBase, IProgress<RomSetGeneratorProgress>
{
    [ObservableProperty]
    private string _currentSystem = string.Empty;

    [ObservableProperty]
    private string _currentFile = string.Empty;

    [ObservableProperty]
    private int _systemProgress;

    [ObservableProperty]
    private int _totalProgress;

    public bool FinishedSuccesfully { get; private set; }

    public RomSetGeneratorResult Result { get; private set; } = new();

    public RomSetGeneratorDialogViewModel(Ra2DatContext context)
    {
        _context = context;
        _romSetGenerator = new RomSetGenerator(context);

        var gamesCount = context.GetCheckedSystems().SelectMany(s => s.GetGamesMatchingFilter(context.UserConfig.GameTypesFilter)).Count();

        CurrentSystem = $"Building set for {gamesCount} games";
    }

    public void Report(RomSetGeneratorProgress value)
    {
        CurrentSystem = $"Processing {value.currentSystem}";
        CurrentFile = value.currentFile;
        SystemProgress = (int)value.systemProgress;
        TotalProgress = (int)value.totalProgress;
    }

    [RelayCommand]
    public async Task Cancel()
    {
        if (_cancellationTokenSrc != null)
        {
            await _cancellationTokenSrc.CancelAsync();
        }
        else
        {
            App.CurrentWindow().Close();
        }

    }

    [RelayCommand]
    public async Task Start()
    {
        _cancellationTokenSrc = new CancellationTokenSource();

        try
        {
            Result = await _romSetGenerator.GenerateSet(this, _cancellationTokenSrc.Token);
            FinishedSuccesfully = true;

        }
        catch (Exception)
        {
            FinishedSuccesfully = false;
        }

        _cancellationTokenSrc.Dispose();
        _cancellationTokenSrc = null;
    }

    private CancellationTokenSource? _cancellationTokenSrc;

    private Ra2DatContext _context;
    private RomSetGenerator _romSetGenerator;
}
