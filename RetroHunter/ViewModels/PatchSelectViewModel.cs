using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;

namespace RetroHunter.ViewModels
{
    public partial class PatchSelectViewModel : ViewModelBase
    {
        [ObservableProperty]
        private FileInfo? _selectedPatch;

        [ObservableProperty]
        private IEnumerable<FileInfo> _patches = [];

        public PatchSelectViewModel(IEnumerable<FileInfo> patches)
        {
            Patches = patches;
        }

        public PatchSelectViewModel()
        { }

        [RelayCommand]
        public void Save()
        {
            App.CurrentWindow().Close();
        }

        [RelayCommand]
        public void Discard()
        {
            SelectedPatch = null;
            App.CurrentWindow().Close();
        }
    }
}
