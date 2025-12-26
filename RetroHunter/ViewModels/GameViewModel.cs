using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RetroHunter.Models;
using RetroHunter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetroHunter.ViewModels
{
    public partial class GameViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Bitmap? _gameIcon;

        [ObservableProperty]
        private Bitmap? _statusIcon;

        [ObservableProperty]
        private string _gameTypes;

        [ObservableProperty]
        private List<RomViewModel> _roms;

        [ObservableProperty]
        private bool _atLeastOnePatch;

        [ObservableProperty]
        private bool _atLeastOneLinkedRom;

        [ObservableProperty]
        private string _toolTipText;

        public Game Game { get; private set; }

        public int ValidRomsCount { get; private set; }

        public GameViewModel(MainViewModel mainVm, Game game) : base()
        {
            Game = game;
            _mainVm = mainVm;
            Roms = Game.Roms.Select(r => new RomViewModel(mainVm, this, r)).OrderBy(r => r.Title).ToList();
            GameTypes = GameTypesToString();
            ToolTipText = "";
            UpdateStatus();
        }

        public GameViewModel() : base()
        { }

        public async Task LoadDetails(CancellationTokenSource? ct)
        {
            GameIcon = await ImageHelper.LoadFromWeb(new Uri(Game.IconUrl));

            if (ct?.IsCancellationRequested ?? false)
            {
                return;
            }

            await _mainVm.UpdateGameData(Game);

            if (ct?.IsCancellationRequested ?? false)
            {
                return;
            }

            Roms = [.. Game.Roms.Select(r => new RomViewModel(_mainVm, this, r))];
        }

        public void UpdateStatus()
        {
            ValidRomsCount = Roms.Sum(rvm => rvm.IsRomValid ? 1 : 0);
            AtLeastOneLinkedRom = Roms.Sum(rvm => rvm.IsRomLinked ? 1 : 0) > 0;

            var iconSrc = "";
            if (!AtLeastOneLinkedRom) {
                iconSrc = "avares://RetroHunter/Assets/error.png";
                ToolTipText = "No roms are linked to this game";
            }
            else if (ValidRomsCount == 0)
            {
                iconSrc = "avares://RetroHunter/Assets/warning.png";
                ToolTipText = "Roms are linked, but rom files are missing at linked path";
            }
            else
            {
                iconSrc = "avares://RetroHunter/Assets/check.png";
            }

            StatusIcon = ImageHelper.LoadFromResource(new Uri(iconSrc));
            AtLeastOnePatch = Roms.Any(rvm => rvm.HasPatch);
        }

        private string GameTypesToString() => string.Join("", Game.GameTypes.Select(t => $"[{t}]").Order());

        private MainViewModel _mainVm;
    }
}
