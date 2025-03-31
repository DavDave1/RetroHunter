using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscUtils.Vfs;
using RaSetMaker.Models;
using RaSetMaker.Persistence;
using RaSetMaker.Services;
using RaSetMaker.Utils;
using RaSetMaker.Views;
using SharpCompress.Common;
using SharpCompress.Readers;

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
            await SelectedGame.LoadDetails(_loadingDetailsCancellation);

            if (_loadingDetailsCancellation.IsCancellationRequested)
            {
                return;
            }
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

    [RelayCommand]
    public async Task LoadModel()
    {
        await FetchUserProfile();

        _raClient.SetApiKey(_dbContext.UserConfig.RaApiKey);
        _chdman.SetChdManPath(_dbContext.UserConfig.ChdmanExePath);

        var systems = _dbContext.GetSystems();

        var companyList = new List<GameSystemCompanyViewModel>();
        foreach (var company in Enum.GetValues<GameSystemCompany>())
        {
            companyList.Add(new(this, company, [.. systems.Where(gs => gs.Company == company).OrderBy(gs => gs.Name)]));
        }

        CompanyList = companyList;
        SelectedSystem = null;
        GamesList = [];
    }

    public async Task ApplyPatch(RomViewModel romViewModel)
    {
        var rom = romViewModel.Rom;

        var patchArchiveFileName = rom.PatchUrl.Substring(rom.PatchUrl.LastIndexOf('/') + 1);
        var patchArchiveFile = Path.Combine(Path.GetTempPath(), patchArchiveFileName);
        var extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(patchArchiveFile));
        Directory.CreateDirectory(extractDir);

        try
        {
            using ScopedTaskProgress progress = new(this, "Downloading patch file...");
            // Download patch file
            {
                HttpClient client = new();
                using var s = await client.GetStreamAsync(rom.PatchUrl);
                using var fs = new FileStream(patchArchiveFile, FileMode.Create);
                await s.CopyToAsync(fs);
                fs.Close();
            }

            // Open file as archive
            using (Stream stream = File.OpenRead(patchArchiveFile))
            using (var reader = ReaderFactory.Open(stream))
            {
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

            var zip = ZipFile.OpenRead(patchArchiveFile);
            var patches = new DirectoryInfo(extractDir).EnumerateFiles("*.bps");

            // TODO: handle multi patch files
            if (patches.Count() > 1)
            {
                await App.ShowInfo("Multiple patch files found",
                $"Multiple patch files found\n {string.Join('\n', patches.Select(p => p.FullName).Where(n => Path.GetExtension(n) == ".bps"))}");

            }

            var patchFile = patches.First().FullName;

            var sourceCrc = await RomPatcher.Patcher.GetSourceCrc32(patchFile);

            // Find source rom
            var sourceRom = rom.Game?.GameSystem?.Games.SelectMany(g => g.Roms).FirstOrDefault(r => r.RomFiles.Any(rf => rf.Crc32 == sourceCrc));

            if (sourceRom == null)
            {
                await App.ShowError("Failed to find source ROM", $"Failed to find source ROM for {rom.RaName}");
                return;
            }

            progress.SetMessage("Applying patch...");
            var generator = new RomSetGenerator(_dbContext);

            var outputDir = _dbContext.UserConfig.OutputRomsDirectory;
            var romSrcPath = Path.Combine(outputDir, sourceRom.RomFiles.First(rf => rf.Crc32 == sourceCrc).FilePath);

            await generator.GenerateFromPatch(romSrcPath, rom, patchFile);

            await App.ShowInfo("Patch applied", $"Succesfully created {rom.RaName} from patch");
        }
        catch (Exception e)
        {
            await App.ShowError("Failed to apply patch", e.Message);
        }
        finally
        {
            Directory.Delete(extractDir, true);
            File.Delete(patchArchiveFile);
        }

    }

    public async Task Compress(TreeViewItemModel vm)
    {
        if (vm is RomViewModel romViewModel)
        {
            var sizeBefore = romViewModel.Rom.GetSize();
            using var progress = new ScopedTaskProgress(this, $"Compressing {romViewModel.Rom.RaName}", 100);

            var sys = _dbContext.GetSystems().First(s => s.Games.FirstOrDefault(g => g.Roms.Contains(romViewModel.Rom)) != null);
            bool ok = await _chdman.CompressRom(_dbContext.UserConfig, sys, romViewModel.Rom, progress);


            if (!ok)
            {
                await App.ShowError("Failed to compress ROM", $"Failed to compress {romViewModel.Rom.RaName}");
            }
            else
            {
                await _dbContext.SaveChangesAsync();
                var ratio = 100 * romViewModel.Rom.GetSize() / (float)sizeBefore;
                await App.ShowInfo("ROM compression completed", $"ROM {romViewModel.Rom.RaName} compressed successfully.\nCompression ratio: {ratio:F1}");
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
        _loadingDetailsCancellation?.Cancel();

        if (_loadingDetailsTask != null && !_loadingDetailsTask.IsCompleted)
        {
            _loadingDetailsTask.Wait();
        }

        _loadingDetailsCancellation = new CancellationTokenSource();

        HasSelectedGame = value != null;
        _loadingDetailsTask = Task.Run(LoadDetails);
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

    private Task _loadingDetailsTask;

    private CancellationTokenSource _loadingDetailsCancellation;
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
