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
        public RomViewModel(MainViewModel mainVm, Rom rom) : base(mainVm)
        {
            Rom = rom;
            Title = rom.RaName;
            _mainVm = mainVm;
        }

        [ObservableProperty]
        private Rom _rom;

        public override string IconSrc => "avares://RaSetMaker/Assets/rom.png";

        public bool IsRomValid => Rom.Exists();

        public bool HasPatch => Rom.PatchUrl != string.Empty;

        [RelayCommand]
        private async Task ApplyPatch()
        {
            await _mainVm.ApplyPatch(this);
        }

        protected override bool CanCompress => true;

        private readonly MainViewModel _mainVm;
    }
}
