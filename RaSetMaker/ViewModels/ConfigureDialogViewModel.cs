using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Models;
using RaSetMaker.Persistence;
using RaSetMaker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaSetMaker.ViewModels
{
    public partial class ConfigureDialogViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _raApiKey = string.Empty;

        [ObservableProperty]
        private string _inputRomsDirectory = string.Empty;

        [ObservableProperty]
        private string _outputRomsDirectory = string.Empty;

        public static List<DirStructureStyle> DirStructureStylesList => [DirStructureStyle.Retroachievements, DirStructureStyle.EmuDeck];

        [ObservableProperty]
        private DirStructureStyle _selectedDirStructureStyle;

        public static List<GameType> GameTypesList => Enum.GetValues<GameType>().Order().ToList();

        [ObservableProperty]
        private List<GameType> _selectedGameTypesFilter = [];

        public bool WasCanceled { get; private set; } = true;

        public ConfigureDialogViewModel(RaClient raClient, Ra2DatContext context)
        {
            _raClient = raClient;
            _context = context;
            UserName = _context.UserConfig.Name;
            RaApiKey = _context.UserConfig.RaApiKey;
            InputRomsDirectory = _context.UserConfig.InputRomsDirectory;
            OutputRomsDirectory = _context.UserConfig.OutputRomsDirectory;
            SelectedDirStructureStyle = _context.UserConfig.DirStructureStyle;
            SelectedGameTypesFilter = _context.UserConfig.GameTypesFilter;
        }

        [RelayCommand]
        public void Save()
        {
            _context.UserConfig.Name = UserName;
            _context.UserConfig.RaApiKey = RaApiKey;
            _context.UserConfig.InputRomsDirectory = InputRomsDirectory;
            _context.UserConfig.OutputRomsDirectory = OutputRomsDirectory;
            _context.UserConfig.DirStructureStyle = SelectedDirStructureStyle;
            _context.UserConfig.GameTypesFilter = SelectedGameTypesFilter;

            _raClient.SetApiKey(_context.UserConfig.RaApiKey);

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
        public async Task BrowseInputRomsDirectory()
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
        public async Task BrowseOutputRomsDirectory()
        {
            // Browse for the output path
            var result = await App.CurrentWindow().StorageProvider.OpenFolderPickerAsync(new() { AllowMultiple = false, Title = "Pick Rom Output Folder"});

            var outputDir = result.FirstOrDefault();

            if (outputDir != null)
            {
               OutputRomsDirectory = outputDir.Path.AbsolutePath.ToString();
            }
        }

        [RelayCommand]
        public async Task TestLogin()
        {
            _raClient.SetApiKey(RaApiKey);
            bool loginResult = await _raClient.TestLogin(UserName);

            if (loginResult)
            {
                await App.ShowInfo("Login successful", "Login successful");
            }
            else
            {
                await App.ShowError("Login failed", "Could not login with provided key, please make sure key is valid");
            }
        }

        private readonly RaClient _raClient;
        private readonly Ra2DatContext _context;
    }
}
