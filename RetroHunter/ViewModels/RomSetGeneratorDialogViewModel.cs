
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroHunter.Persistence;
using RetroHunter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetroHunter.ViewModels;

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

    [ObservableProperty]
    private List<string> _exceptions = [];

    public bool FinishedSuccesfully { get; private set; }

    public RomSetGeneratorResult Result { get; private set; } = new();

    public RomSetGeneratorDialogViewModel(Ra2DatContext context)
    {
        _romSetGenerator = new RomSetGenerator(context);

        var gamesCount = context.GetCheckedSystems().SelectMany(s => s.GetGamesMatchingFilter()).Count();

        CurrentSystem = $"Building set for {gamesCount} games";
    }

    public void Report(RomSetGeneratorProgress value)
    {
        CurrentSystem = value.currentSystem;
        CurrentFile = value.currentFile;
        SystemProgress = (int)value.systemProgress;
        TotalProgress = (int)value.totalProgress;
        Exceptions = value.exceptions;
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
        catch (Exception ex)
        {
            await App.ShowError("Rom Set Generation Failed", ex.Message);
            FinishedSuccesfully = false;
        }

        _cancellationTokenSrc.Dispose();
        _cancellationTokenSrc = null;
        App.CurrentWindow().Close();
    }

    private CancellationTokenSource? _cancellationTokenSrc;

    private RomSetGenerator _romSetGenerator;
}
