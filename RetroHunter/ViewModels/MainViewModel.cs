using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RetroHunter.Models;
using RetroHunter.Persistence;
using RetroHunter.Services;
using RetroHunter.Utils;
using RetroHunter.Views;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace RetroHunter.ViewModels;

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
    private Bitmap? _userIcon = ImageHelper.LoadFromResource(new Uri("avares://RetroHunter/Assets/question.png"));

    [ObservableProperty]
    private GameViewModel? _selectedGame;

    [ObservableProperty]
    public GameViewModel? _detailGame;

    [ObservableProperty]
    private bool _hasSelectedGame = false;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvier = serviceProvider;
        _dbContext = serviceProvider.GetService<Ra2DatContext>()!;
        _raClient = serviceProvider.GetService<RaClient>()!;
        _settingsManager = serviceProvider.GetService<SettingsManager>()!;
        _compressFactory = serviceProvider.GetService<CompressServiceFactory>()!;
        _matcherFactory = serviceProvider.GetService<MatcherFactory>()!;

        var systems = _dbContext.GetSystems().ToList();
        var companyList = new List<GameSystemCompanyViewModel>();
        foreach (var company in Enum.GetValues<GameSystemCompany>())
        {
            companyList.Add(new(this, company, [.. systems.Where(gs => gs.Company == company).OrderBy(gs => gs.Name)]));
        }
        CompanyList = companyList;
    }

    /// <summary>
    /// Used for designer
    /// </summary>
    public MainViewModel()
    {
        _settingsManager = new(null);
        _dbContext = new();
        _raClient = new(_settingsManager);
    }

    [RelayCommand]
    private async Task OpenDatabase()
    {
        try
        {

            var dialog = new NewProjectDialog();

            var vm = new NewProjectDialogViewModel(_settingsManager);
            dialog.DataContext = vm;

            await dialog.ShowDialog<NewProjectDialogViewModel?>(App.MainWindow());
            if (!vm.WasCanceled)
            {
                await _dbContext.LoadModelAsync(vm.ProjectFilePath, vm.NewProject);
                await LoadModel();
            }
        }
        catch(Exception e)
        {
            await App.ShowError("Failed to load project", e.Message);
        }
    }

    [RelayCommand]
    public async Task Configure()
    {
        var dialog = new ConfigureDialog();

        var vm = new ConfigureDialogViewModel(_raClient, _dbContext, _settingsManager);
        dialog.DataContext = vm;

        await dialog.ShowDialog<ConfigureDialogViewModel?>(App.CurrentWindow());

        if (!vm.WasCanceled)
        {
            await _settingsManager.Save();

            _dbContext.ValidateRoms();
            await _dbContext.SaveChangesAsync();
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

            await _dbContext.SaveChangesAsync();
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
        var vm = _serviceProvier.GetService<RomSetGeneratorDialogViewModel>();
        Debug.Assert(vm != null);

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


        await _dbContext.SaveChangesAsync();
        await LoadModel();
    }

    public async Task LoadDetails()
    {
        if (SelectedGame != null)
        {
            await SelectedGame.LoadDetails(_loadingDetailsCancellation);

            if (_loadingDetailsCancellation?.IsCancellationRequested ?? false)
            {
                return;
            }
            DetailGame = SelectedGame;
        }
    }


    public async Task FetchUserProfile()
    {
        if (_settingsManager.Settings.RaName == string.Empty)
        {
            return;
        }

        try
        {
            UserProfile = await _raClient.GetUserProfile();
            UserIcon = await ImageHelper.LoadFromWeb(new Uri(UserProfile.UserPic));

        }
        catch (Exception ex)
        {
            await App.ShowError("Failed to fetch user profile", ex.Message);
        }
    }

    [RelayCommand]
    public async Task LoadModel()
    {
        try
        {
            if (_dbContext.FilePath == string.Empty)
            {
                var dialog = new NewProjectDialog();

                var vm = new NewProjectDialogViewModel(_settingsManager);
                dialog.DataContext = vm;

                await dialog.ShowDialog<NewProjectDialogViewModel?>(App.MainWindow());

                if (!vm.WasCanceled)
                {
                    await _dbContext.LoadModelAsync(vm.ProjectFilePath, vm.NewProject);
                }
                else
                {
                    App.CurrentWindow().Close();
                }
            }

            var systems = _dbContext.GetSystems().ToList();
            CompanyList.ForEach(cvm => cvm.RefreshModel([.. systems.Where(gs => gs.Company == cvm.Company).OrderBy(gs => gs.Name)]));

            SelectedSystem = null;
            GamesList = [];

            await FetchUserProfile();
        }
        catch (Exception e)
        {
            await App.ShowError("Failed to load project", e.Message);
        }
    }

    public async Task ApplyPatch(RomViewModel romViewModel)
    {
        var rom = romViewModel.Rom;

        try
        {
            using ScopedTaskProgress progress = new(this, "Downloading patch file...");

            var patchArchiveFile = await _raClient.DownloadPatch(rom.PatchUrl);

            var extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(patchArchiveFile));
            Directory.CreateDirectory(extractDir);

            // Open file as archive
            {
                using Stream stream = File.OpenRead(patchArchiveFile);
                using var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(extractDir, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }

            var patches = new DirectoryInfo(extractDir)
                .EnumerateFiles($"{Path.GetFileNameWithoutExtension(rom.RaName)}.bps");

            if (!patches.Any())
            {
                patches = new DirectoryInfo(extractDir)
                .EnumerateFiles("*.bps");
            }

            var patchFile = "";
            if (patches.Count() > 1)
            {
                var patchSelectVm = new PatchSelectViewModel([.. patches]);
                var dialog = new PatchSelectDialog
                {
                    DataContext = patchSelectVm
                };


                await dialog.ShowDialog<PatchSelectViewModel?>(App.MainWindow());

                if (patchSelectVm.SelectedPatch == null)
                {
                    return;
                }

                patchFile = patchSelectVm.SelectedPatch.FullName;

            }
            else
            {
                patchFile = patches.First().FullName;
            }

            var sourceCrc = await RomPatcher.Patcher.GetSourceCrc32(patchFile);

            // Find source rom
            var sourceRom = rom.Game?.GameSystem?.Games.SelectMany(g => g.Roms).FirstOrDefault(r => r.RomFiles.Any(rf => rf.Crc32 == sourceCrc));

            if (sourceRom == null)
            {
                await App.ShowError("Failed to find source ROM", $"Failed to find source ROM for {rom.RaName}");

                Directory.Delete(extractDir, true);
                File.Delete(patchArchiveFile);

                return;
            }

            progress.SetMessage("Applying patch...");
            var generator = new RomSetGenerator(_dbContext, _matcherFactory, _settingsManager);

            var romSrcPath = sourceRom.RomFiles.First(rf => rf.Crc32 == sourceCrc);

            await RomSetGenerator.GenerateFromPatch(romSrcPath, rom, patchFile);
            await _dbContext.SaveChangesAsync();

            Directory.Delete(extractDir, true);
            File.Delete(patchArchiveFile);

            await App.ShowInfo("Patch applied", $"Succesfully created {rom.RaName} from patch");

        }
        catch (Exception e)
        {
            await App.ShowError("Failed to apply patch", e.Message);
        }
    }

    public async Task Compress(Rom rom)
    {
        using var progress = new ScopedTaskProgress(this, $"Compressing {rom.RaName}", 100);

        try
        {
            var sizeBefore = rom.GetSize();

            var compressor = _compressFactory.CreateCompressService(_settingsManager.Settings, rom);

            await compressor.CompressRom(_dbContext.UserConfig, rom, progress);
            await _dbContext.SaveChangesAsync();

            var ratio = 100 * rom.GetSize() / (float)sizeBefore;
            await App.ShowInfo("ROM compression completed", $"ROM {rom.RaName} compressed successfully.\nCompression ratio: {ratio:F1}");
        }
        catch(Exception ex)
        {
            await App.ShowError($"Failed to compress {rom.RaName}", ex.Message);
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
        _loadingDetailsCancellation?.Cancel();

        if (_loadingDetailsTask != null && !_loadingDetailsTask.IsCompleted)
        {
            _loadingDetailsTask.Wait();
        }

        _loadingDetailsCancellation = new CancellationTokenSource();

        HasSelectedGame = value != null;
        _loadingDetailsTask = Task.Run(LoadDetails);
    }

    private readonly IServiceProvider _serviceProvier;
    private readonly Ra2DatContext _dbContext;
    private readonly RaClient _raClient;
    private readonly SettingsManager _settingsManager;
    private readonly CompressServiceFactory _compressFactory;
    private readonly MatcherFactory _matcherFactory;

    private static readonly FilePickerFileType RaSetMakerDb = new("RetroHunter DB")
    {
        Patterns = new[] { "*.json" },
        AppleUniformTypeIdentifiers = new[] { "public.json" },
        MimeTypes = new[] { "json/*" }
    };

    private Task? _loadingDetailsTask;

    private CancellationTokenSource? _loadingDetailsCancellation;
}

internal class ScopedTaskProgress : IDisposable, IProgress<CompressProgress>
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

    public void Report(CompressProgress value)
    {
        _progress = value.Percent;
        _vm.ProgressValue = (int)_progress;
    }

    private readonly MainViewModel _vm;
    private int _count;
    private double _step;
    private double _progress;
}
