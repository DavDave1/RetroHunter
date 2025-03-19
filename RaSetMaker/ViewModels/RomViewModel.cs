using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaSetMaker.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RaSetMaker.ViewModels
{
    public partial class RomViewModel : TreeViewItemModel
    {
        public RomViewModel(MainViewModel mainVm, Rom rom, UserConfig userConfig) : base(mainVm)
        {
            Rom = rom;
            Title = rom.RaName;
            _userConfig = userConfig;
            _mainVm = mainVm;
        }

        [ObservableProperty]
        private Rom _rom;

        public override string IconSrc => "avares://RaSetMaker/Assets/rom.png";

        public bool IsRomValid => Rom.Exists(_userConfig.OutputRomsDirectory);

        public bool HasPatch => Rom.PatchUrl != string.Empty;

        public string RomName => _fInfo != null ? Path.GetFileNameWithoutExtension(_fInfo.FullName) : string.Empty;

        [RelayCommand]
        private async Task ApplyPatch()
        {
            await _mainVm.ApplyPatch(this);
        }

        protected override bool CanCompress => true;

        private FileInfo? _fInfo => Rom.FilePaths.Count == 0 ? null : new FileInfo(Path.Combine(_userConfig.OutputRomsDirectory, Rom.FilePaths.First()));

        private readonly UserConfig _userConfig;

        private readonly MainViewModel _mainVm;
    }
}
