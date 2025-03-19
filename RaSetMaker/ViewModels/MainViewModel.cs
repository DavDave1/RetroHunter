using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Models;
using RaSetMaker.Persistence;
using RaSetMaker.Services;
using RaSetMaker.Utils;
using RaSetMaker.Views;

namespace RaSetMaker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public List<GameSystemCompanyViewModel> _companyList = [];

    [ObservableProperty]
    private List<GameViewModel> _gamesList = [];

    [ObservableProperty]
    private GameSystemViewModel? _selectedSystem;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _progressIndeterminate = false;

    [ObservableProperty]
    private int _progressValue = 0;

    [ObservableProperty]
    private RaUserProfile _userProfile = new();

    [ObservableProperty]
    private Bitmap? _userIcon = ImageHelper.LoadFromResource(new Uri("avares://RaSetMaker/Assets/question.png"));

    [ObservableProperty]
    private GameViewModel? _selectedGame;

    [ObservableProperty]
    public GameViewModel? _detailGame;

    [ObservableProperty]
    private bool _hasSelectedGame = false;

    public MainViewModel(RaClient raClient, Chdman chdman, Ra2DatContext context)
    {
        _dbContext = context;
        _raClient = raClient;
        _chdman = chdman;

    }

    /// <summary>
    /// Used for designer
    /// </summary>
    public MainViewModel()
    {
        _dbContext = new();
        _raClient = new();
    }

    [RelayCommand]
    private async Task OpenDatabase()
    {
        var opts = new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Open RaSetMaker DB file",
            FileTypeFilter = [RaSetMakerDb],
            SuggestedFileName = "RaSetMaker.xml",

        };
        var result = await App.CurrentWindow().StorageProvider.OpenFilePickerAsync(opts);

        var dbFile = result.FirstOrDefault();

        if (dbFile != null)
        {
            _dbContext.LoadModel(dbFile.Path.AbsolutePath);
            _raClient.SetApiKey(_dbContext.UserConfig.RaApiKey);
            await LoadModel();
        }
    }

    [RelayCommand]
    private async Task SaveDatabase()
    {
        if (_dbContext.FilePath == string.Empty)
        {
            var opts = new FilePickerSaveOptions()
            {
                Title = "Save RaSetMaker DB File As",
                FileTypeChoices = [RaSetMakerDb],
                SuggestedFileName = "RaSetMaker.xml",
                ShowOverwritePrompt = true,
            };

            var dbFile = await App.CurrentWindow().StorageProvider.SaveFilePickerAsync(opts);

            if (dbFile == null)
            {
                return;
            }

            _dbContext.FilePath = dbFile.Path.AbsolutePath;
        }

        await _dbContext.SaveChangesAsync();
    }

    [RelayCommand]
    public async Task Configure()
    {
        var dialog = new ConfigureDialog();

        var vm = new ConfigureDialogViewModel(_raClient, _chdman, _dbContext);
        dialog.DataContext = vm;

        await dialog.ShowDialog<ConfigureDialogViewModel?>(App.CurrentWindow());

        if (!vm.WasCanceled)
        {
            _dbContext.ValidateRoms();
        }

        await LoadModel();
    }


    [RelayCommand]
    public async Task FetchRaData()
    {
        using var progress = new ScopedTaskProgress(this, "Fetching systems...");

        try
        {
            var now = DateTime.UtcNow;
            var systems = _dbContext.GetSystems().Where(s => now.Subtract(s.LastUpdate).Days >= 7).ToList();
            progress.SetCount(systems.Count);

            foreach (var system in systems)
            {
                progress.SetMessage($"Fetching games for {system.Name}...");

                var fetchedGames = await _raClient.FetchGames(system);
                _dbContext.SyncGames(fetchedGames, system);
                await Task.Delay(500);
                progress.Step();
            }

            await App.ShowInfo("Data Fetching Successfull", "All sistems succesfully updated.");

        }
        catch (Exception e)
        {
            await App.ShowError("Failed to fetch RetroAchievement data", e.Message);
        }

        await LoadModel();
    }

    [RelayCommand]
    public async Task GenerateSets()
    {
        var vm = new RomSetGeneratorDialogViewModel(_dbContext);
        var dialog = new RomSetGeneratorProgressDialog
        {
            DataContext = vm
        };

        await dialog.ShowDialog<RomSetGeneratorProgressDialog?>(App.CurrentWindow());

        if (vm.FinishedSuccesfully)
        {
            await App.ShowInfo("Rom Set Generation Complete",
                $"removed {vm.Result.RemovedRoms} roms and added {vm.Result.AddedRoms} roms");

        }
        else
        {
            await App.ShowError("Rom Set Generation Finished with error",
                $"removed {vm.Result.RemovedRoms} roms and added {vm.Result.AddedRoms} roms");
        }

        await LoadModel();
    }

    public async Task LoadDetails()
    {
        if (SelectedGame != null)
        {
            await SelectedGame.LoadDetails();
            DetailGame = SelectedGame;
        }
    }


    public async Task FetchUserProfile()
    {
        if (_dbContext.UserConfig.Name == string.Empty)
        {
            return;
        }

        try
        {
            UserProfile = await _raClient.GetUserProfile(_dbContext.UserConfig.Name);
            UserIcon = await ImageHelper.LoadFromWeb(new Uri(UserProfile.UserPic));

        }
        catch (Exception ex)
        {
            await App.ShowError("Failed to fetch user profile", ex.Message);
        }
    }

    public async Task LoadModel()
    {
        await FetchUserProfile();

        _raClient.SetApiKey(_dbContext.UserConfig.RaApiKey);
        _chdman.SetChdManPath(_dbContext.UserConfig.ChdmanExePath);

        var systems = _dbContext.GetSystems();

        var companyList = new List<GameSystemCompanyViewModel>();
        foreach (var company in Enum.GetValues<GameSystemCompany>())
        {
            companyList.Add(new(this, company, [.. systems.Where(gs => gs.Company == company).OrderBy(gs => gs.Name)], _dbContext.UserConfig));
        }

        CompanyList = companyList;
        SelectedSystem = null;
        GamesList = [];
    }

    public async Task ApplyPatch(RomViewModel romViewModel)
    {
        await App.ShowInfo("Apply Patch", romViewModel.Rom.PatchUrl);

    }

    public async Task Compress(TreeViewItemModel vm)
    {
        if (vm is RomViewModel romViewModel)
        {
            var sizeBefore = romViewModel.Rom.GetSize(_dbContext.UserConfig.OutputRomsDirectory);
            using var progress = new ScopedTaskProgress(this, $"Compressing {romViewModel.RomName}", 100);

            var sys = _dbContext.GetSystems().First(s => s.Games.FirstOrDefault(g => g.Roms.Contains(romViewModel.Rom)) != null);
            bool ok = await _chdman.CompressRom(_dbContext.UserConfig, sys, romViewModel.Rom, progress);


            if (!ok)
            {
                await App.ShowError("Failed to compress ROM", $"Failed to compress {romViewModel.RomName}");
            }
            else
            {
                await _dbContext.SaveChangesAsync();
                var ratio = 100 * romViewModel.Rom.GetSize(_dbContext.UserConfig.OutputRomsDirectory) / (float)sizeBefore;
                await App.ShowInfo("ROM compression completed", $"ROM {romViewModel.RomName} compressed successfully.\nCompression ratio: {ratio:F1}");
            }

        }
    }

    public async Task UpdateGameData(Game game)
    {
        await _raClient.UpdateGame(game);
    }

    partial void OnSelectedSystemChanged(GameSystemViewModel? value)
    {
        GamesList = value?.Games.ToList() ?? [];
    }

    partial void OnSelectedGameChanging(GameViewModel? value)
    {
        HasSelectedGame = value != null;
        Task.Run(LoadDetails);
    }

    private readonly Ra2DatContext _dbContext;
    private readonly RaClient _raClient;
    private readonly Chdman _chdman;

    private static readonly FilePickerFileType RaSetMakerDb = new("RaSetMaker DB")
    {
        Patterns = new[] { "*.xml" },
        AppleUniformTypeIdentifiers = new[] { "public.xml" },
        MimeTypes = new[] { "xml/*" }
    };
}

internal class ScopedTaskProgress : IDisposable, IProgress<ChdmanProgress>
{
    public ScopedTaskProgress(MainViewModel vm, string message, int count = 0)
    {
        _vm = vm;
        SetCount(count);
        SetMessage(message);
    }

    public void SetCount(int count)
    {
        _count = count;
        _step = 100.0 / _count;
        _vm.ProgressIndeterminate = count <= 0;
        _vm.ProgressValue = 0;
        _progress = 0;
    }

    public void Step()
    {
        _progress += _step;
        _vm.ProgressValue = (int)_progress;
    }

    public void SetMessage(string message)
    {
        _vm.StatusMessage = message;
    }

    public void Dispose()
    {
        _vm.StatusMessage = "";
        _vm.ProgressValue = 0;
        _vm.ProgressIndeterminate = false;
    }

    public void Report(ChdmanProgress value)
    {
        _progress = value.Percent;
        _vm.ProgressValue = (int)_progress;
    }

    private readonly MainViewModel _vm;
    private int _count;
    private double _step;
    private double _progress;
}
