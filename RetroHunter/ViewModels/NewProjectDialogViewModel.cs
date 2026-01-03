using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroHunter.Services;

namespace RetroHunter.ViewModels;

public partial class NewProjectDialogViewModel(SettingsManager settingsManager) : ViewModelBase
{
    [ObservableProperty]
    private bool _wasCanceled = true;

    [ObservableProperty]
    private string _projectFilePath = "";

    [ObservableProperty]
    private bool _newProject;

    [RelayCommand]
    private async Task OpenExisting()
    {
        var opts = new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Open RetroHunter DB file",
            FileTypeFilter = [RetroHunterDb],
            SuggestedFileName = "RetroHunter.json",

        };
        var result = await App.CurrentWindow().StorageProvider.OpenFilePickerAsync(opts);

        var dbFile = result.FirstOrDefault();

        if (dbFile != null)
        {
            await UpdateLatestProjectPath(dbFile.Path.AbsolutePath);
            App.CurrentWindow().Close();
        }
    }

    [RelayCommand]
    private async Task CreateNew()
    {

        var opts = new FilePickerSaveOptions()
        {
            Title = "Create RetroHunter DB file",
            FileTypeChoices = [RetroHunterDb],
            SuggestedFileName = "RetroHunter.json",

        };
        var result = await App.CurrentWindow().StorageProvider.SaveFilePickerAsync(opts);

        if (result != null)
        {
            NewProject = true;
            await UpdateLatestProjectPath(result.Path.AbsolutePath);
            App.CurrentWindow().Close();
        }
    }

    [RelayCommand]
    private async Task OpenLatest()
    {
        ProjectFilePath = settingsManager.Settings.LatestDbPath;
        if (ProjectFilePath != string.Empty)
        {

            WasCanceled = false;
            App.CurrentWindow().Close();
        }
    }

    private static readonly FilePickerFileType RetroHunterDb = new("RetroHunter DB")
    {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["json/*"]
    };

    private async Task UpdateLatestProjectPath(string path)
    {

        ProjectFilePath = path;
        WasCanceled = false;
        settingsManager.Settings.LatestDbPath = ProjectFilePath;
        await settingsManager.Save();
    }
}
