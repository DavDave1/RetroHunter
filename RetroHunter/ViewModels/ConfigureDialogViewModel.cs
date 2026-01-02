using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroHunter.Models;
using RetroHunter.Persistence;
using RetroHunter.Services;
using RetroHunter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroHunter.ViewModels
{
    public partial class ConfigureDialogViewModel : ViewModelBase
    {
        public static List<DirStructureStyle> DirStructureStylesList => [DirStructureStyle.Retroachievements, DirStructureStyle.EmuDeck];

        public static List<GameType> GameTypesList => Enum.GetValues<GameType>().Order().ToList();

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _raApiKey = string.Empty;

        [ObservableProperty]
        private string _inputRomsDirectory = string.Empty;

        [ObservableProperty]
        private string _outputRomsDirectory = string.Empty;

        [ObservableProperty]
        private DirStructureStyle _selectedDirStructureStyle;

        [ObservableProperty]
        private List<GameType> _selectedGameTypesFilter = [];

        [ObservableProperty]
        private string _chdmanExePath = string.Empty;

        [ObservableProperty]
        private string _dolphinToolExePath = string.Empty;

        public bool WasCanceled { get; private set; } = true;

        public ConfigureDialogViewModel(RaClient raClient, Ra2DatContext context, SettingsManager settingsManager)
        {
            _raClient = raClient;
            _context = context;
            _settingsManager = settingsManager;
            UserName = _settingsManager.Settings.RaName;
            RaApiKey = _settingsManager.Settings.RaApiKey;
            InputRomsDirectory = _context.UserConfig.InputRomsDirectory;
            OutputRomsDirectory = _context.UserConfig.OutputRomsDirectory;
            SelectedDirStructureStyle = _context.UserConfig.DirStructureStyle;
            SelectedGameTypesFilter = _context.UserConfig.GameTypesFilter;
            ChdmanExePath = _settingsManager.Settings.ChdmanExePath;
            DolphinToolExePath  = _settingsManager.Settings.DolphinToolExePath;
        }

        [RelayCommand]
        public void Save()
        {
            _settingsManager.Settings.RaName = UserName;
            _settingsManager.Settings.RaApiKey = RaApiKey;
            _settingsManager.Settings.ChdmanExePath = ChdmanExePath;
            _settingsManager.Settings.DolphinToolExePath = DolphinToolExePath;
            _context.UserConfig.InputRomsDirectory = InputRomsDirectory;
            _context.UserConfig.OutputRomsDirectory = OutputRomsDirectory;
            _context.UserConfig.DirStructureStyle = SelectedDirStructureStyle;
            _context.UserConfig.GameTypesFilter = SelectedGameTypesFilter;

            WasCanceled = false;
            App.CurrentWindow().Close();
        }

        [RelayCommand]
        public void Discard()
        {
            WasCanceled = true;
            App.CurrentWindow().Close();
        }

        [RelayCommand]
        private async Task BrowseInputRomsDirectory()
        {
            // Browse for the output path
            var result = await App.CurrentWindow().StorageProvider.OpenFolderPickerAsync(new() { AllowMultiple = false, Title = "Pick Rom Input Folder" });

            var inputDir = result.FirstOrDefault();

            if (inputDir != null)
            {
                InputRomsDirectory = inputDir.Path.AbsolutePath.ToString();
            }
        }

        [RelayCommand]
        private async Task BrowseOutputRomsDirectory()
        {
            // Browse for the output path
            var result = await App.CurrentWindow().StorageProvider.OpenFolderPickerAsync(new() { AllowMultiple = false, Title = "Pick Rom Output Folder" });

            var outputDir = result.FirstOrDefault();

            if (outputDir != null)
            {
                OutputRomsDirectory = outputDir.Path.AbsolutePath.ToString();
            }
        }

        [RelayCommand]
        private async Task TestLogin()
        {
            bool loginResult = await _raClient.TestLogin(UserName, RaApiKey);

            if (loginResult)
            {
                await App.ShowInfo("Login successful", "Login successful");
            }
            else
            {
                await App.ShowError("Login failed", "Could not login with provided key, please make sure key is valid");
            }
        }

        [RelayCommand]
        private async Task DetectChdman()
        {
            var chdmanPath = DirUtils.FindTool("chdman");
            if (string.IsNullOrEmpty(chdmanPath))
            {
                await App.ShowError("Chdman not found", "Chdman not found in PATH");
            }
            else
            {
                await App.ShowInfo("Chdman detected", $"Chdman found at {chdmanPath}");
                ChdmanExePath = chdmanPath;
            }
        }

        [RelayCommand]
        private async Task DetectDolphinTool()
        {
            var dolphinToolPath = DirUtils.FindTool("DolphinTool");
            if (string.IsNullOrEmpty(dolphinToolPath))
            {
                await App.ShowError("DolphinTool not found", "DolphinTool not found in PATH");
            }
            else
            {
                await App.ShowInfo("DolphinTool detected", $"DolphinTool found at {dolphinToolPath}");
                DolphinToolExePath = dolphinToolPath;
            }
        }

        private readonly RaClient _raClient;
        private readonly Ra2DatContext _context;
        private readonly SettingsManager _settingsManager;
    }
}
