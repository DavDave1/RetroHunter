using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetroHunter.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetroHunter.ViewModels
{
    public partial class RomViewModel : TreeViewItemModel
    {
        public RomViewModel(MainViewModel mainVm, GameViewModel parent, Rom rom) : base()
        {
            Rom = rom;
            Title = rom.RaName;
            _mainVm = mainVm;
            Parent = parent;
            IsRomValid = Rom.Exists();
            IsRomLinked = Rom.RomFiles.Count > 0;
        }

        [ObservableProperty]
        private Rom _rom;

        [ObservableProperty]
        private bool _isRomValid;

        [ObservableProperty]
        private bool _isRomLinked;

        public GameViewModel Parent;

        public bool HasPatch => Rom.PatchUrl != string.Empty;

        [RelayCommand]
        private async Task ApplyPatch()
        {
            await _mainVm.ApplyPatch(this);
            IsRomValid = Rom.Exists();
            Parent.UpdateStatus();
        }

        [RelayCommand]
        private async Task Compress()
        {
            await _mainVm.Compress(Rom);
        }

        [RelayCommand]
        private async Task OpenInExplorer()
        {
            var romDir = new FileInfo(Rom.RomFiles.First().AbsolutePath()).Directory;
            await App.MainWindow().Launcher.LaunchDirectoryInfoAsync(romDir);
        }

        protected override bool CanCompress => true;

        private readonly MainViewModel _mainVm;
    }
}
