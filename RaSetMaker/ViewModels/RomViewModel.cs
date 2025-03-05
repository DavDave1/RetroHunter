using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using RaSetMaker.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RaSetMaker.ViewModels
{
    public partial class RomViewModel(MainViewModel mainVm, Rom rom, UserConfig userConfig) : TreeViewItemModel(mainVm)
    {
        public override string Title => RomName == string.Empty ? Rom.Hash : RomName;

        [ObservableProperty]
        private Rom _rom = rom;

        public override string IconSrc => "avares://RaSetMaker/Assets/rom.png";

        public bool IsRomValid() => Rom.Exists(userConfig.OutputRomsDirectory);

        public string RomName => _fInfo != null ? Path.GetFileNameWithoutExtension(_fInfo.FullName) : string.Empty;

        protected override bool CanCompress => true;

        private readonly FileInfo? _fInfo = rom.FilePaths.Count == 0 ? null : new FileInfo(Path.Combine(userConfig.OutputRomsDirectory, rom.FilePaths.First()));

    }
}
