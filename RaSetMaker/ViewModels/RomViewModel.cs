using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Models;
using System.IO;

namespace RaSetMaker.ViewModels
{
    public partial class RomViewModel(Rom rom) : TreeViewItemModel
    {
        public override string Title => $"{RomName} ({Rom.Hash})";

        public override string ForegroundColor => IsRomValid() ? "Black" : "DarkRed";

        [ObservableProperty]
        private Rom _rom = rom;

        public bool IsRomValid() => _fileInfo != null && _fileInfo.Exists;

        private FileInfo? _fileInfo = rom.FilePath == string.Empty ? null : new FileInfo(rom.FilePath);

        private string RomName => IsRomValid() ? _fileInfo!.Name : Rom.Name;

    }
}
