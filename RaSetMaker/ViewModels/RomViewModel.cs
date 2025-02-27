using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RaSetMaker.ViewModels
{
    public partial class RomViewModel(Rom rom, UserConfig userConfig) : TreeViewItemModel
    {
        public override string Title => RomName == string.Empty ? Rom.Hash : RomName;

        [ObservableProperty]
        private Rom _rom = rom;

        public override string IconSrc => "avares://RaSetMaker/Assets/rom.png";

        public bool IsRomValid() => Rom.Exists(userConfig.OutputRomsDirectory);

        public string RomName => rom.FilePaths.Count > 1 ? _fInfo?.Directory?.Name ?? "" : _fInfo?.Name ?? "";


        private readonly FileInfo? _fInfo = rom.FilePaths.Count == 0 ? null : new FileInfo(Path.Combine(userConfig.OutputRomsDirectory, rom.FilePaths.First()));

    }
}
