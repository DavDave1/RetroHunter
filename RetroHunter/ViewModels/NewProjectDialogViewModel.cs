﻿using System.Linq;
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
            await UpdateLatestProjectPath(result.Path.AbsolutePath);
            App.CurrentWindow().Close();
        }
    }

    [RelayCommand]
    private async Task OpenLatest()
    {
        _settings = await settingsManager.Load();

        ProjectFilePath = _settings.LatestDbPath;
        if (ProjectFilePath != string.Empty)
        {

            WasCanceled = false;
            App.CurrentWindow().Close();
        }
    }

    private static readonly FilePickerFileType RetroHunterDb = new("RetroHunter DB")
    {
        Patterns = new[] { "*.json" },
        AppleUniformTypeIdentifiers = new[] { "public.json" },
        MimeTypes = new[] { "json/*" }
    };

    private async Task UpdateLatestProjectPath(string path)
    {

        ProjectFilePath = path;
        WasCanceled = false;
        _settings.LatestDbPath = ProjectFilePath;
        await settingsManager.Save(_settings);
    }

    private RetroHunterSettings _settings = new();
}
